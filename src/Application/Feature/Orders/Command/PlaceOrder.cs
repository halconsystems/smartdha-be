using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Command;

public class OrderItemDto
{
    [Required]
    public Guid ItemId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}
public class OrderPlaceCommand : IRequest<SuccessResponse<string>>
{
    [Required]
    public OrderType OrderType { get; set; }
    [Required]
    public Guid ServiceId { get; set; }
    [Required]
    public Guid PackageId { get; set; }
    [Required]
    public List<OrderItemDto> ItemsId { get; set; } = default!;

    [Required]
    public string OrderCity { get; set; } = default!;
    [Required]
    public string CompleteAddress { get; set; } = default!; 
    public string? NearByLandMark { get; set; }
    [Required]
    public string PhoneNumber { get; set; } = default!; 
    public string? DeliveryInstruction { get; set; }
    [Required]
    public PaymentMethod PaymentMethod { get; set; }
    public string? BascketId { get; set; }

    [Required]
    public string PickupAddress { get; set; } = default!;

    [Required]
    public string DeliverAddress { get; set; } = default!;

    [Required]
    public Guid ShopId { get; set; } = default!;

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Confirmed;


}
public class OrderPlaceCommandHandler : IRequestHandler<OrderPlaceCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderPlaceCommandHandler(ILaundrySystemDbContext context, IFileStorageService fileStorageService, ISmsService otpService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _fileStorage = fileStorageService;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<string>> Handle(OrderPlaceCommand command, CancellationToken ct)
    {
        try
        {
            decimal taxex = 0;
            decimal discount = 0;
            decimal totalAmount = 0;
            decimal subtotal = 0;
            decimal remaining = 0;



            var today = DateTime.UtcNow.ToString("yyyyMMdd");

            string UsedId = _currentUserService.UserId.ToString();

            //Order Tax and Discount Details Created By Application Owner

            var orderDTDetails = _context.OrderDTSettings
                .AsNoTracking()
                .ToList();


            taxex = orderDTDetails.Where(x => x.IsDiscount == false && x.DTCode != "HAN").Sum(x => Convert.ToInt32(x.Value));


            discount = orderDTDetails.Where(x => x.IsDiscount).Sum(x => Convert.ToInt32(x.Value));

            totalAmount = totalAmount + taxex - discount;

            //Orders Items Details
            var itemsDetails = _context.LaundryItems
                .Where(x => command.ItemsId.Select(i => i.ItemId).Contains(x.Id))
                .Select(x => new
                {
                    ItemId = x.Id,
                    Price = x.ItemPrice
                })
                .ToDictionary(x => x.ItemId, x => x.Price);

            //Laundry Items After Hanger Price Adjustment

            var laundryItemsByPackageId = await _context.LaundryPackagings.FirstOrDefaultAsync(x => x.Id == command.PackageId);

            decimal HangerPrice = 0;
            HangerPrice = laundryItemsByPackageId == null ? 0 : orderDTDetails.Where(x => x.IsDiscount == false && x.DTCode == "HAN").Sum(x => Convert.ToInt32(x.Value));


            //Order Unique Number
            var lastOrder = await _context.Orders
            .Where(x => x.UniqueFormID.StartsWith($"Order-{today}"))
            .OrderByDescending(x => x.UniqueFormID)
            .Select(x => x.UniqueFormID)
            .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastOrder))
            {
                var lastSequence = lastOrder.Split('-').Last(); // 000009
                nextNumber = int.Parse(lastSequence) + 1;
            }

            var sequence = nextNumber.ToString("D6"); // 000010

            //Payment Check Method for Online

            OrderPaymentIpnLogs? OnlinePaymentLogs = null;
            if (command.PaymentMethod == PaymentMethod.Online)
            {
                if (string.IsNullOrWhiteSpace(command.BascketId))
                    throw new KeyNotFoundException("BasketId is required for online payment.");

                OnlinePaymentLogs = await _context.OrderPaymentIpnLogs.FirstOrDefaultAsync(x => x.BasketId == command.BascketId);

                if (OnlinePaymentLogs == null)
                    throw new KeyNotFoundException("Invalid or unpaid BasketId.");
            }

            //OrderSummaries Set

            var orderSummaries = command.ItemsId.Select(x =>
            {
                var itemPrice = Convert.ToDecimal(itemsDetails[x.ItemId]) + HangerPrice;
                var itemTotal = itemPrice * x.Quantity;

                totalAmount += itemTotal;

                return new OrderSummary
                {
                    ItemId = x.ItemId,
                    ItemCount = x.Quantity.ToString(),
                    ItemPrice = itemPrice.ToString(),
                    TotalCountPrice = itemTotal.ToString()
                };
            }).ToList();



            var entity = new Domain.Entities.LMS.Orders
            {
                ServiceId = command.ServiceId,
                PackageId = command.PackageId,
                UserId = UsedId,
                OrderType = command.OrderType,
                UniqueFormID = $"Order-{today}-{sequence}",
                PickUpAddress = command.PickupAddress,
                DeliverAddress = command.DeliverAddress,
                ShopId = command.ShopId,
                OrderStatus = command.OrderStatus,
                AmountToCollect = command.PaymentMethod == PaymentMethod.Cash ? totalAmount : 0,
                CollectedAmount = command.PaymentMethod == PaymentMethod.Cash ? 0 : OnlinePaymentLogs?.TransactionAmount,
            };

            _context.Orders.Add(entity);
            await _context.SaveChangesAsync(ct);


            foreach (var summary in orderSummaries)
            {
                summary.OrderId = entity.Id;
            }

            _context.OrderSummaries.AddRange(orderSummaries);
            await _context.SaveChangesAsync(ct);

            subtotal = orderSummaries.Sum(x => Convert.ToDecimal(x.TotalCountPrice));
            remaining = orderSummaries.Sum(x => Convert.ToDecimal(x.TotalCountPrice));


            var DiscountTaxDetails = await _context.OrderDTSettings
                .Where(x => x.DTCode != "Han")
                .AsNoTracking()
                .ToListAsync();

            var orderpaymentDTSetting = DiscountTaxDetails.Select(x => new PaymentDTSetting
            {
                OrderId = entity.Id,
                OrderDTiD = x.Id,
                IsDiscount = x.IsDiscount
            }).ToList();

            _context.PaymentDTSettings.AddRange(orderpaymentDTSetting);
            await _context.SaveChangesAsync(ct);

            var deliveryEntity = new DeliveryDetails
            {
                OrderId = entity.Id,
                City = command.OrderCity,
                CompleteAddress = command.CompleteAddress,
                NearByLandMark = command.NearByLandMark,
                PhoneNumber = command.PhoneNumber,
                DeliveryInstruction = command.DeliveryInstruction,
                subTotal = subtotal,
                Total = totalAmount,
                Taxes = taxex,
                Discount = discount,
                PaymentMethod = command.PaymentMethod,
                paidAmount = command.PaymentMethod == PaymentMethod.Cash ? entity.CollectedAmount : totalAmount,
                RemainingBalance = totalAmount - (command.PaymentMethod == PaymentMethod.Cash ? entity.CollectedAmount : totalAmount),
            };

            _context.DeliveryDetails.Add(deliveryEntity);
            await _context.SaveChangesAsync(ct);



            return Success.Created(entity.Id.ToString());
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }
}


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
using Microsoft.AspNetCore.Http;

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
}
public class OrderPlaceCommandHandler : IRequestHandler<OrderPlaceCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUserService;

    public OrderPlaceCommandHandler(ILaundrySystemDbContext context, IFileStorageService fileStorageService, ISmsService otpService, ICurrentUserService currentUserService)
    {
        _context = context;
        _fileStorage = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(OrderPlaceCommand command, CancellationToken ct)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");

        string UsedId = _currentUserService.UserId.ToString();
        var lastOrder = await _context.Orders
        .Where(x => x.UniqueFormID.StartsWith($"PAN-{today}"))
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


        var entity = new Domain.Entities.LMS.Orders
        {
            ServiceId = command.ServiceId,
            PackageId = command.PackageId,
            UserId = UsedId,
            OrderType = command.OrderType,
            UniqueFormID = $"Order-{today}-{sequence}",
        };

        _context.Orders.Add(entity);
        await _context.SaveChangesAsync(ct);

        var itemsDetails = _context.LaundryItems
    .Where(x => command.ItemsId.Select(i => i.ItemId).Contains(x.Id))
    .Select(x => new
    {
        ItemId = x.Id,
        Price = x.ItemPrice
    })
    .ToDictionary(x => x.ItemId, x => x.Price);


        var ordersummary = command.ItemsId.Select(x => new OrderSummary
        {
            OrderId = entity.Id,
            ItemId = x.ItemId,
            ItemCount = x.Quantity.ToString(),
            ItemPrice = itemsDetails[x.ItemId],
            TotalCountPrice = (Convert.ToInt32(itemsDetails[x.ItemId]) * x.Quantity).ToString(),
        }).ToList();

        _context.OrderSummaries.AddRange(ordersummary);
        await _context.SaveChangesAsync(ct);


        var DiscountTaxDetails = await _context.OrderDTSettings
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

        decimal taxex = 0; 
        decimal discount = 0; 
        decimal totalAmount = 0;
        decimal subtotal = 0;
        decimal remaining = 0;

        var orderDTDetails = _context.PaymentDTSettings
            .Where(x => x.OrderId == entity.Id)
            .Include(x => x.OrderDTSetting)
            .AsNoTracking()
            .ToList();

        taxex = orderDTDetails.Where(x => x.IsDiscount == false).Sum(x => Convert.ToInt32(x.OrderDTSetting?.Value));
        discount = orderDTDetails.Where(x => x.IsDiscount).Sum(x => Convert.ToInt32(x.OrderDTSetting?.Value));

        totalAmount = ordersummary.Sum(x => Convert.ToDecimal(x.TotalCountPrice)) + taxex - discount;
        subtotal = ordersummary.Sum(x => Convert.ToDecimal(x.TotalCountPrice));
        remaining = ordersummary.Sum(x => Convert.ToDecimal(x.TotalCountPrice));


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
            Taxes  = taxex,
            Discount = discount,
            PaymentMethod = command.PaymentMethod,
            //paidAmount = command.PaymentMethod == PaymentMethod.Cash ? 0 : 
        };

        _context.DeliveryDetails.Add(deliveryEntity);
        await _context.SaveChangesAsync(ct);

        return Success.Created(entity.Id.ToString());
    }
}


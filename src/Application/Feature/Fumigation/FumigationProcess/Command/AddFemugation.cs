using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.FMS;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;
public class AddFemugationCommand : IRequest<SuccessResponse<string>>
{
    public Guid FemPhaseId { get; set; }
    public Guid FemServiceId { get; set; }
    public Guid FemTanker { get; set; }
    [Required]
    public string StreetNo { get; set; } = default!;
    [Required]
    public string PhoneNumber { get; set; } = default!;
    [Required]
    public string Address { get; set; } = default!;
    [Required]
    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? BasketId { get; set; }
    public Guid ShopId { get; set; }
}

public class AddFemugationCommandHandler : IRequestHandler<AddFemugationCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddFemugationCommandHandler(IApplicationDbContext context , ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(AddFemugationCommand command, CancellationToken ct)
    {
        try
        {
            decimal taxex = 0;
            decimal discount = 0;
            decimal totalAmount = 0;



            var today = DateTime.UtcNow.ToString("yyyyMMdd");

            var UsedId = _currentUserService.UserId;

            //Order Tax and Discount Details Created By Application Owner

            var FMDTDetails = _context.FemDTSettings
                .AsNoTracking()
                .ToList();


            taxex = FMDTDetails.Where(x => x.IsDiscount == false).Sum(x => Convert.ToInt32(x.Value));


            discount = FMDTDetails.Where(x => x.IsDiscount).Sum(x => Convert.ToInt32(x.Value));

            totalAmount = totalAmount + taxex - discount;

            


            //Order Unique Number
            var lastOrder = await _context.Fumigations
            .Where(x => x.CaseNo.StartsWith($"FM-{today}"))
            .OrderByDescending(x => x.CaseNo)
            .Select(x => x.CaseNo)
            .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastOrder))
            {
                var lastSequence = lastOrder.Split('-').Last(); // 000009
                nextNumber = int.Parse(lastSequence) + 1;
            }

            var sequence = nextNumber.ToString("D6"); // 000010

            //Payment Check Method for Online

            FemPaymentIpnLogs? OnlinePaymentLogs = null;
            if (command.PaymentMethod == PaymentMethod.Online)
            {
                if (string.IsNullOrWhiteSpace(command.BasketId))
                    throw new KeyNotFoundException("BasketId is required for online payment.");

                OnlinePaymentLogs = await _context.FemPaymentIpnLogs.FirstOrDefaultAsync(x => x.BasketId == command.BasketId);

                if (OnlinePaymentLogs == null)
                    throw new KeyNotFoundException("Invalid or unpaid BasketId.");
            }

            var sizePrice = await _context.TankerSizes.FirstOrDefaultAsync(x => x.Id == command.FemTanker);

            if(sizePrice == null) throw new KeyNotFoundException("Size Price Not Found.");

            totalAmount = Convert.ToDecimal(sizePrice.Price) - taxex + discount;

            var entity = new Domain.Entities.FMS.Fumigation
            {
                FemPhaseID = command.FemPhaseId,
                FemServiceId = command.FemServiceId,
                FemTanker = command.FemTanker,
                UserId = UsedId,
                StreetNo = command.StreetNo,
                CaseNo = $"FM-{today}-{sequence}",
                PhoneNumber = command.PhoneNumber,
                Address = command.Address,
                ShopId = command.ShopId,
                DateOnly = command.DateOnly,
                TimeOnly = command.TimeOnly,
                AmountToCollect = command.PaymentMethod == PaymentMethod.Cash ? totalAmount : 0,
                CollectedAmount = command.PaymentMethod == PaymentMethod.Cash ? 0 : OnlinePaymentLogs?.TransactionAmount,
                SubTotal = Convert.ToDecimal(sizePrice.Price),
                Taxes = taxex,
                Discount = discount,
                Total = totalAmount,
                PaymentMethod = command.PaymentMethod
            };

            _context.Fumigations.Add(entity);
            await _context.SaveChangesAsync(ct);


            return Success.Created(entity.Id.ToString());
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }
}


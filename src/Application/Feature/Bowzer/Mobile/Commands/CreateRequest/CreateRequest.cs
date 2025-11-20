using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts;
using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;

using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ReqStatus = DHAFacilitationAPIs.Domain.Enums.BowserStatus;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.CreateRequest;

public sealed record CreateRequestResultDto(
    Guid RequestId,
    string RequestNo,
    string Status,
    string PaymentStatus,
    string PaymentId,         // 12-digit unique
    decimal Amount,
    string Currency,
    string Phase,
    string CapacityLabel,
    DateTime RequestedDeliveryDate
);

public sealed record CreateRequestCommand(CreateRequestDto Payload) : IRequest<CreateRequestResultDto>;
public sealed class CreateRequestHandler(
    IOLHApplicationDbContext db,
    ICurrentUserService currentUser,UserManager<ApplicationUser> _userManager,ISmartPayService _smartPayService,ILogger<CreateRequestHandler> _logger)
    : IRequestHandler<CreateRequestCommand, CreateRequestResultDto>
{
    public async Task<CreateRequestResultDto> Handle(CreateRequestCommand request, CancellationToken ct)
    {
        var p = request.Payload;
        var userId = currentUser.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");

        var clickedUser = await _userManager.FindByIdAsync(userId);


        var now = DateTime.Now;

        // Pull everything we need in ONE query: availability + rate + labels
        var pc = await db.PhaseCapacities
            .AsNoTracking()
            .Where(x =>
                x.PhaseId == p.PhaseId &&
                x.BowserCapacityId == p.CapacityId &&
                (x.IsDeleted == false || x.IsDeleted == null) &&
                x.EffectiveFrom <= now &&
                (x.EffectiveTo == null || x.EffectiveTo >= now))
            .Select(x => new
            {
                x.BaseRate,
                PhaseName = x.Phase.Name,
                CapValue = x.BowserCapacity.Capacity,
                CapUnit = x.BowserCapacity.Unit
            })
            .FirstOrDefaultAsync(ct);

        if (pc is null)
            throw new InvalidOperationException("Selected capacity is not available for the chosen phase.");

        // Compute amount/currency (ignore input)
        var amount = pc.BaseRate ?? 0m;
        var currency = "PKR";
        var capLabel = $"{pc.CapValue} {pc.CapUnit}";

        // Unique IDs
        var requestNo = NextRequestNo();
        var paymentId = SmartPayBillId.GenerateOneBillId("6010"); // ensure uniqueness against existing payments

        var entity = new OLH_BowserRequest
        {
            RequestNo = requestNo,
            RequestDate = now,

            PhaseId = p.PhaseId,
            BowserCapacityId = p.CapacityId,

            RequestedDeliveryDate = p.RequestedDeliveryDate,   // assume already UTC or handle conversion
            PlannedDeliveryDate = null,
            DeliveryDate = null,

            Ext = p.Ext,
            Street = p.Street,
            Address = p.Address,
            Latitude = p.Latitude,
            Longitude = p.Longitude,

            Amount = amount,
            Currency = currency,
            Status = ReqStatus.Draft,
            PaymentStatus = PaymentStatus.Pending,  // requires your enum to include Pending

            CustomerId = userId,
            Notes = p.Notes,

            Created = now,
            LastModified = now,
            IsDeleted = false,
            IsActive = true
        };

        db.BowserRequests.Add(entity);

        // (Optional but recommended) create a Payment row now with the generated PaymentId
        // If you don't have OLH_Payment, rename to your Payment entity.
        db.Payments.Add(new OLH_Payment
        {
            RequestId = entity.Id,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Pending,
            Provider = "SMART-PAY",                 // or your default
            ProviderPaymentId = paymentId,            // store the 12-digit ID here
            CreatedAt = now,

            Created = now,
            LastModified = now,
            IsActive = true,
            IsDeleted = false
        });

        await db.SaveChangesAsync(ct);

        // SmartPay bill upload (only this block)
        var uploadRequest = new SmartPayUploadBillRequest
        {
            Consumer_Number = paymentId,                   // the OneBill ID you generated
            Consumer_Detail = clickedUser?.Name ?? "",                     // OR member name
            Billing_Month = DateTime.Now.ToString("yyMM"),
            DueDate = DateTime.Now.AddMinutes(30).ToString("yyyyMMddhhmmss"),
            ExpDate = DateTime.Now.AddMinutes(30).ToString("yyyyMMddhhmmss"),
            Amount = amount,                      // base rate (your deposit/amount)
            LateFee = 0,
            CellNo = clickedUser?.RegisteredMobileNo ?? "",
            EMail = clickedUser?.Email ?? "",
            ReferenceInfo = $"Bowser-Request"
        };

        try
        {
            await _smartPayService.UploadBillAsync(uploadRequest, ct);
        }
        catch (Exception ex)
        {
            // throw new InvalidOperationException("SmartPay bill upload failed: " + ex.Message);
            _logger.LogError(ex, "SmartPay Bowzer upload failed for RequestId: {RequestId}", paymentId);
        }


        // Return receipt-ready data for the app
        return new CreateRequestResultDto(
            entity.Id,
            entity.RequestNo,
            entity.Status.ToString(),
            entity.PaymentStatus.ToString(),
            paymentId,
            entity.Amount,
            entity.Currency,
            pc.PhaseName,
            capLabel,
            entity.RequestedDeliveryDate
        );
    }

    // Readable request number (timestamp + 6-hex suffix)
    private static string NextRequestNo()
    {
        var now = DateTime.Now;
        Span<byte> rnd = stackalloc byte[3]; // 3 bytes → 6 hex chars
        RandomNumberGenerator.Fill(rnd);
        var suffix = Convert.ToHexString(rnd);
        return $"BZR-{now:yyyyMMdd-HHmmssfff}-{suffix}";
    }

    // 12-digit numeric PaymentId with DB uniqueness check
    
}

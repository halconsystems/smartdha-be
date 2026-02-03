using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Application.Feature.PaymentBills.Commands.AddPayLateFeePolicy;
public record CreatePayLateFeePolicyCommand(
    string SourceSystem,
    int DueAfterDays,
    int GraceDays,
    int ExpireAfterDays,
    LateFeeType LateFeeType,
    decimal FixedLateFee,
    decimal PerDayLateFee
) : IRequest<ApiResult<Guid>>;
public class CreatePayLateFeePolicyHandler
    : IRequestHandler<CreatePayLateFeePolicyCommand, ApiResult<Guid>>
{
    private readonly IPaymentDbContext _db;

    public CreatePayLateFeePolicyHandler(IPaymentDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreatePayLateFeePolicyCommand r,
        CancellationToken ct)
    {
        // ❌ Prevent duplicate active policy per source system
        var exists = await _db.Set<PayLateFeePolicy>()
            .AnyAsync(x =>
                x.SourceSystem == r.SourceSystem &&
                x.IsActive==true,
                ct);

        if (exists)
            return ApiResult<Guid>.Fail(
                $"Late fee policy already exists for {r.SourceSystem}");

        var policy = new PayLateFeePolicy
        {
            SourceSystem = r.SourceSystem,

            DueAfterDays = r.DueAfterDays,
            GraceDays = r.GraceDays,
            ExpireAfterDays = r.ExpireAfterDays,

            LateFeeType = r.LateFeeType,
            FixedLateFee = r.FixedLateFee,
            PerDayLateFee = r.PerDayLateFee,

            IsActive = true,
            Created = DateTime.Now
        };

        _db.PayLateFeePolicies.Add(policy);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(policy.Id, "Late fee policy created successfully.");
    }
}


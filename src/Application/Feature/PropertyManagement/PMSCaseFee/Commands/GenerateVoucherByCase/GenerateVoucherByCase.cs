using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.GenerateVoucherByCase;

public record GenerateVoucherByCaseCommand(
    Guid CaseId,
    decimal? OverrideAmount,
    string? Remarks
) : IRequest<ApiResult<Guid>>;

public class GenerateVoucherByCaseHandler
    : IRequestHandler<GenerateVoucherByCaseCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public GenerateVoucherByCaseHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        GenerateVoucherByCaseCommand request,
        CancellationToken ct)
    {
        var c = await _db.Set<PropertyCase>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(x => x.Id == request.CaseId, ct);

        if (c == null)
            return ApiResult<Guid>.Fail("Case not found.");

        var caseFee = await _db.Set<CaseFee>()
            .Include(x => x.FeeDefinition)
            .FirstOrDefaultAsync(x => x.CaseId == request.CaseId, ct);

        if (caseFee == null)
            return ApiResult<Guid>.Fail("Case fee not calculated.");

        if (request.OverrideAmount.HasValue)
        {
            if (!caseFee.FeeDefinition!.AllowOverride)
                return ApiResult<Guid>.Fail("Fee override not allowed.");

            caseFee.Amount = request.OverrideAmount.Value;
            caseFee.IsOverridden = true;
        }

        var voucher = new CaseVoucher
        {
            CaseId = request.CaseId,
            Amount = caseFee.Amount,
            Status = VoucherStatus.Generated,
            VoucherNo = $"V-{DateTime.UtcNow:yyyyMMddHHmmss}",
            GeneratedAt = DateTime.UtcNow,
            Remarks = request.Remarks
        };

        _db.Set<CaseVoucher>().Add(voucher);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(voucher.Id, "Voucher generated.");
    }
}

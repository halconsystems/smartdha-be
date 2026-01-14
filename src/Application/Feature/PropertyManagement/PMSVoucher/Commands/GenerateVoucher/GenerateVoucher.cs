using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSVoucher.Commands.GenerateVoucher;
public record GenerateVoucherCommand(Guid CaseId, decimal Amount, string? Remarks) : IRequest<ApiResult<Guid>>;

public class GenerateVoucherHandler : IRequestHandler<GenerateVoucherCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public GenerateVoucherHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(GenerateVoucherCommand r, CancellationToken ct)
    {
        var c = await _db.Set<PropertyCase>().FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);
        if (c == null) return ApiResult<Guid>.Fail("Case not found.");

        if (c.CurrentStepId == null) return ApiResult<Guid>.Fail("Case not in workflow.");

        var step = await _db.Set<ProcessStep>().FirstAsync(x => x.Id == c.CurrentStepId, ct);
        if (!step.RequiresVoucher) return ApiResult<Guid>.Fail("Voucher is not required at this step.");

        var voucherNo = $"V-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var v = new CaseVoucher
        {
            CaseId = c.Id,
            Status = VoucherStatus.Generated,
            VoucherNo = voucherNo,
            Amount = r.Amount,
            GeneratedAt = DateTime.UtcNow,
            GeneratedAtStepId = step.Id,
            Remarks = r.Remarks
        };

        _db.Set<CaseVoucher>().Add(v);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(v.Id, "Voucher generated.");
    }
}


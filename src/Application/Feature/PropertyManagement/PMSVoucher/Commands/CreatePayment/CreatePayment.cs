using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSVoucher.Commands.CreatePayment;
public record CreatePaymentCommand(
    Guid CaseId,
    Guid? VoucherId,
    PaymentMethod Method,
    decimal Amount,
    string? TransactionId,
    string? CollectedBy
) : IRequest<ApiResult<Guid>>;

public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreatePaymentHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreatePaymentCommand r, CancellationToken ct)
    {
        var c = await _db.Set<PropertyCase>().AnyAsync(x => x.Id == r.CaseId, ct);
        if (!c) return ApiResult<Guid>.Fail("Case not found.");

        if (r.VoucherId.HasValue)
        {
            var voucher = await _db.Set<CaseVoucher>().FirstOrDefaultAsync(x => x.Id == r.VoucherId, ct);
            if (voucher == null) return ApiResult<Guid>.Fail("Voucher not found.");
        }

        var p = new CasePayment
        {
            CaseId = r.CaseId,
            VoucherId = r.VoucherId,
            Method = r.Method,
            Amount = r.Amount,
            TransactionId = r.TransactionId,
            CollectedBy = r.CollectedBy,
            Status = PaymentStatus.Success,
            PaidAt = DateTime.UtcNow
        };

        _db.Set<CasePayment>().Add(p);

        // Update voucher status if any
        if (r.VoucherId.HasValue)
        {
            var v = await _db.Set<CaseVoucher>().FirstAsync(x => x.Id == r.VoucherId, ct);
            v.Status = VoucherStatus.Paid;
        }

        await _db.SaveChangesAsync(ct);
        return ApiResult<Guid>.Ok(p.Id, "Payment recorded.");
    }
}


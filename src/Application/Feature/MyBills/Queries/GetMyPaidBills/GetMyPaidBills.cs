using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetMyPaidBills;
public record GetMyPaidBillsQuery : IRequest<List<MyPaidBillSummaryDto>>;

public class GetMyPaidBillsHandler
    : IRequestHandler<GetMyPaidBillsQuery, List<MyPaidBillSummaryDto>>
{
    private readonly IPaymentDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMyPaidBillsHandler(
        IPaymentDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<List<MyPaidBillSummaryDto>> Handle(
        GetMyPaidBillsQuery request,
        CancellationToken ct)
    {
        var userId = _current.UserId!.ToString();

        return await _db.PayBills
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.PaymentStatus == PaymentBillStatus.Paid)
            .OrderByDescending(x => x.LastPaymentDate)
            .Select(x => new MyPaidBillSummaryDto
            {
                PaymentBillId = x.Id,
                Title = x.Title,
                SourceVoucherNo = x.SourceVoucherNo,
                SourceSystem = x.SourceSystem,
                PaymentStatus = x.PaymentStatus,

                BillAmount = x.BillAmount,
                PaidAmount = x.PaidAmount,
                Paymentdate=x.LastPaymentDate

            })
            .ToListAsync(ct);
    }
}

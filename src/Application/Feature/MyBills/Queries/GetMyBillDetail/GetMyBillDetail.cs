
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetMyBillDetail;
public record GetMyBillDetailQuery(Guid PaymentBillId)
    : IRequest<MyBillDetailDto>;
public class GetMyBillDetailHandler
    : IRequestHandler<GetMyBillDetailQuery, MyBillDetailDto>
{
    private readonly IPaymentDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMyBillDetailHandler(
        IPaymentDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<MyBillDetailDto> Handle(
        GetMyBillDetailQuery request,
        CancellationToken ct)
    {
        var userId = _current.UserId!.ToString();

        var bill = await _db.PayBills
            .Include(x => x.Transactions)
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.PaymentBillId == request.PaymentBillId &&
                x.UserId == userId,
                ct);

        if (bill == null)
            throw new NotFoundException("Bill not found.");

        return new MyBillDetailDto
        {
            PaymentBillId = bill.PaymentBillId,
            Title = bill.Title,
            SourceSystem = bill.SourceSystem,
            EntityName = bill.EntityName,

            BillAmount = bill.BillAmount,
            PaidAmount = bill.PaidAmount,
            OutstandingAmount = bill.OutstandingAmount,

            BillGeneratedOn = bill.BillGeneratedOn,
            DueDate = bill.DueDate,
            ExpiryDate = bill.ExpiryDate,

            PaymentStatus = bill.PaymentStatus,

            Transactions = bill.Transactions
                .OrderByDescending(t => t.InitiatedAt)
                .Select(t => new MyBillTransactionDto
                {
                    Amount = t.AttemptAmount,
                    Status = t.Status.ToString(),
                    GatewayTransactionId = t.GatewayTransactionId,
                    InitiatedAt = t.InitiatedAt,
                    CompletedAt = t.CompletedAt
                })
                .ToList()
        };
    }
}



using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
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
                x.Id == request.PaymentBillId &&
                x.UserId == userId,
                ct);

        if (bill == null)
            throw new NotFoundException("Bill not found.");

        // ----------------------------------------------------
        // STEP 1: Get latest valid transaction
        // ----------------------------------------------------
        var latestTransaction = bill.Transactions?
            .Where(t => !string.IsNullOrEmpty(t.GatewayTransactionId))
            .OrderByDescending(t => t.InitiatedAt)
            .FirstOrDefault();

        // ----------------------------------------------------
        // STEP 2: Get PaymentName + IssuerName from IPN logs
        // ----------------------------------------------------
        string? paymentName = null;
        string? issuerName = null;

        if (latestTransaction?.GatewayTransactionId != null)
        {
            var ipn = await _db.Set<PaymentIpnLog>()
                .AsNoTracking()
                .Where(x => x.TransactionId == latestTransaction.GatewayTransactionId)
                .OrderByDescending(x => x.Created) // latest IPN
                .Select(x => new
                {
                    x.PaymentName,
                    x.IssuerName
                })
                .FirstOrDefaultAsync(ct);

            if (ipn != null)
            {
                paymentName = ipn.PaymentName;
                issuerName = ipn.IssuerName;
            }
        }

        // ----------------------------------------------------
        // STEP 3: Map response DTO
        // ----------------------------------------------------
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

            Transactions = latestTransaction == null
                ? new MyBillTransactionDto()
                : new MyBillTransactionDto
                {
                    Amount = latestTransaction.AttemptAmount,
                    Status = latestTransaction.Status.ToString(),
                    GatewayTransactionId = latestTransaction.GatewayTransactionId,
                    InitiatedAt = latestTransaction.InitiatedAt,
                    CompletedAt = latestTransaction.CompletedAt,
                    PaymentName = paymentName ?? string.Empty,
                    IssuerName = issuerName ?? string.Empty
                }
        };
    }

}


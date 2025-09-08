using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetMyRequests;
public sealed record GetMyRequestsQuery : IRequest<List<MyRequestHistoryCardDto>>;

public sealed class GetMyRequestsHandler(IOLHApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMyRequestsQuery, List<MyRequestHistoryCardDto>>
{
    public async Task<List<MyRequestHistoryCardDto>> Handle(GetMyRequestsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");

        // Correlated scalar subqueries for PaymentId/Provider (1 payment per request in your design)
        // 1) Project only SQL-friendly members
        var rows = await db.BowserRequests
            .AsNoTracking()
            .Where(r => r.CustomerId == userId && !(r.IsDeleted ?? false))
            .OrderByDescending(r => r.RequestDate)
            .Select(r => new
            {
                r.Id,
                r.RequestNo,
                r.RequestDate,
                r.RequestedDeliveryDate,
                Phase = r.Phase.Name,
                CapValue = r.BowserCapacity.Capacity,
                CapUnit = r.BowserCapacity.Unit,
                r.Status,          // enum (OK to project)
                r.PaymentStatus,   // enum (OK to project)
                r.Amount,
                r.Currency,
                PaymentId = db.Payments
                    .Where(p => p.RequestId == r.Id && !(p.IsDeleted ?? false))
                    .Select(p => p.ProviderPaymentId)
                    .FirstOrDefault(),
                Provider = db.Payments
                    .Where(p => p.RequestId == r.Id && !(p.IsDeleted ?? false))
                    .Select(p => p.Provider)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        // 2) Now do formatting and enum ToString() in memory
        var cards = rows.Select(x => new MyRequestHistoryCardDto(
            RequestId: x.Id,
            RequestNo: x.RequestNo,
            RequestDate: x.RequestDate,
            RequestedDeliveryDate: x.RequestedDeliveryDate,
            Phase: x.Phase,
            CapacityLabel: $"{x.CapValue} {x.CapUnit}",
            Status: x.Status.ToString(),
            PaymentStatus: x.PaymentStatus.ToString(),
            Amount: x.Amount,
            Currency: x.Currency,
            PaymentId: x.PaymentId,
            Provider: x.Provider
        )).ToList();


        return cards;
    }
}

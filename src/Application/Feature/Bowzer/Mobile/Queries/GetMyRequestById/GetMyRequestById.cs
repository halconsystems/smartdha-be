using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetMyRequestById;
public sealed record GetMyRequestByIdQuery(Guid RequestId) : IRequest<MyRequestHistoryCardDto>;
public sealed class GetMyRequestByIdHandler(
    IOLHApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetMyRequestByIdQuery, MyRequestHistoryCardDto>
{
    public async Task<MyRequestHistoryCardDto> Handle(GetMyRequestByIdQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId.ToString()
                     ?? throw new UnauthorizedAccessException("Invalid user.");

        // Query single record
        var result = await db.BowserRequests
            .AsNoTracking()
            .Where(r => r.Id == request.RequestId &&
                        r.CustomerId == userId &&
                        !(r.IsDeleted ?? false))
            .Select(r => new
            {
                r.Id,
                r.RequestNo,
                r.RequestDate,
                r.RequestedDeliveryDate,
                Phase = r.Phase.Name,
                CapValue = r.BowserCapacity.Capacity,
                CapUnit = r.BowserCapacity.Unit,
                r.Status,
                r.PaymentStatus,
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
            .FirstOrDefaultAsync(ct);

        if (result is null)
            throw new KeyNotFoundException("Request not found.");

        // Mapping
        return new MyRequestHistoryCardDto(
            RequestId: result.Id,
            RequestNo: result.RequestNo,
            RequestDate: result.RequestDate,
            RequestedDeliveryDate: result.RequestedDeliveryDate,
            Phase: result.Phase,
            CapacityLabel: $"{result.CapValue} {result.CapUnit}",
            Status: result.Status.ToString(),
            PaymentStatus: result.PaymentStatus.ToString(),
            Amount: result.Amount,
            Currency: result.Currency,
            PaymentId: result.PaymentId,
            Provider: result.Provider
        );
    }
}

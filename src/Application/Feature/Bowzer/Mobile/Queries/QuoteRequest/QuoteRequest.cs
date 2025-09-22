using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.QuoteRequest;
public sealed record QuoteRequestQuery(QuoteRequestDto Payload) : IRequest<QuoteResponseDto>;

public sealed class QuoteRequestHandler(IOLHApplicationDbContext db)
    : IRequestHandler<QuoteRequestQuery, QuoteResponseDto>
{
    public async Task<QuoteResponseDto> Handle(QuoteRequestQuery request, CancellationToken ct)
    {
        var p = request.Payload;
        var now = DateTime.UtcNow;

        // Find Phase×Capacity entry
        var pc = await db.PhaseCapacities
            .Include(x => x.BowserCapacity)
            .FirstOrDefaultAsync(x =>
                x.PhaseId == p.PhaseId &&
                x.BowserCapacityId == p.CapacityId &&
                (x.IsDeleted == false || x.IsDeleted == null) &&
                x.EffectiveFrom <= now &&
                (x.EffectiveTo == null || x.EffectiveTo >= now), ct);

        if (pc is null)
            throw new InvalidOperationException("Selected capacity is not available for the chosen phase.");

        // Business rules – simple example:
        var minLead = 120; // minutes (could be configurable per Phase)
        var isAllowed = p.RequestedDeliveryDate >= now.AddMinutes(minLead);
        var amount = pc.BaseRate ?? 0m;

        return new QuoteResponseDto(
            Amount: amount,
            Currency: "PKR",
            IsAllowed: isAllowed,
            MinLeadTimeMinutes: minLead);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.SubmitRequest;
public sealed record SubmitRequestCommand(Guid RequestId) : IRequest<string>; // returns new status string

public sealed class SubmitRequestHandler(
    IOLHApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<SubmitRequestCommand, string>
{
    public async Task<string> Handle(SubmitRequestCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");

        // No Include(...) needed now; Status is an enum on the same entity
        var entity = await db.BowserRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId && r.CustomerId == userId, ct);

        if (entity is null)
            throw new KeyNotFoundException("Request not found.");

        var toStatus = BowserStatus.Submitted;
        var now = DateTime.Now;

        // Write enum-based history
        db.BowserRequestStatusHistorys.Add(new Domain.Entities.OLH_BowserRequestStatusHistory
        {
            RequestId = entity.Id,
            FromStatus = entity.Status,
            ToStatus = toStatus,
            ChangedAt = now,
            ChangedBy = userId
        });

        // Apply new status
        entity.Status = toStatus;
        entity.LastModified = now;

        await db.SaveChangesAsync(ct);

        // Return string for mobile
        return toStatus.ToString();
    }
}

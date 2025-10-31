using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.BowserStatuses.Commands;
public sealed record UpdateBowserStatusCommand(Guid RequestId, BowserStatus NewStatus)
    : IRequest<string>;

public sealed class UpdateBowserStatusHandler : IRequestHandler<UpdateBowserStatusCommand, string>
{
    private readonly IOLHApplicationDbContext _context;

    public UpdateBowserStatusHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateBowserStatusCommand request, CancellationToken cancellationToken)
    {
        var requestEntity = await _context.BowserRequests
            .FirstOrDefaultAsync(x => x.Id == request.RequestId && x.IsDeleted != true, cancellationToken);

        if (requestEntity == null)
            throw new ArgumentException($"Bowser Request with ID '{request.RequestId}' not found.");

        // Allowed status progression sequence
        var validStatuses = new[]
        {
            BowserStatus.Accepted,
            BowserStatus.Dispatched,
            BowserStatus.OnRoute,
            BowserStatus.Arrived,
            BowserStatus.Delivering,
            BowserStatus.Completed
        };

        // Validate if new status is part of the allowed sequence
        if (!validStatuses.Contains(request.NewStatus))
            throw new InvalidOperationException($"Status '{request.NewStatus}' cannot be set manually.");

        var currentStatusIndex = Array.IndexOf(validStatuses, requestEntity.Status);
        var newStatusIndex = Array.IndexOf(validStatuses, request.NewStatus);

        if (currentStatusIndex == -1)
            throw new InvalidOperationException($"Cannot update from current status '{requestEntity.Status}'.");

        // Ensure the update follows exact next status only
        if (newStatusIndex != currentStatusIndex + 1)
            throw new InvalidOperationException($"Invalid transition: Cannot move from '{requestEntity.Status}' to '{request.NewStatus}' directly.");

        // Update fields
        requestEntity.Status = request.NewStatus;
        requestEntity.LastModified = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return $"Bowser Request '{requestEntity.RequestNo}' updated to status '{request.NewStatus}'.";
    }
}

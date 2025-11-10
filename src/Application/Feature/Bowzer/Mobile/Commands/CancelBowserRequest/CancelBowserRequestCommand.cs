using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.CancelBowserRequest;
public sealed record CancelBowserRequestCommand(Guid RequestId, string? Remarks) : IRequest<string>;

public sealed class CancelBowserRequestHandler : IRequestHandler<CancelBowserRequestCommand, string>
{
    private readonly IOLHApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CancelBowserRequestHandler(IOLHApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(CancelBowserRequestCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");

        var bowserRequest = await _context.BowserRequests
            .FirstOrDefaultAsync(x => x.Id == request.RequestId && x.IsDeleted != true, ct);

        if (bowserRequest is null)
            throw new ArgumentException($"Bowser Request with ID '{request.RequestId}' not found.");

        // Disallowed statuses (cannot be cancelled)
        var nonCancellableStatuses = new[]
        {
            BowserStatus.Accepted,
            BowserStatus.Dispatched,
            BowserStatus.OnRoute,
            BowserStatus.Arrived,
            BowserStatus.Delivering,
            BowserStatus.Completed,
            BowserStatus.Cancelled,
            BowserStatus.Failed
        };

        if (nonCancellableStatuses.Contains(bowserRequest.Status))
            throw new InvalidOperationException(
                $"Cannot cancel a request in '{bowserRequest.Status}' state."
            );

        // Update status
        bowserRequest.Status = BowserStatus.Cancelled;
        bowserRequest.LastModified = DateTime.UtcNow;
        bowserRequest.LastModifiedBy = userId;

        if (!string.IsNullOrWhiteSpace(request.Remarks))
            bowserRequest.Notes = $"{bowserRequest.Notes}\n[Cancelled]: {request.Remarks}".Trim();

        await _context.SaveChangesAsync(ct);

        return $"Bowser Request '{bowserRequest.RequestNo}' has been cancelled successfully.";
    }
}

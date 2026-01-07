using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web.Commands.UpdateComplaintStatus;
public record UpdateComplaintStatusCommand(Guid ComplaintId, ComplaintStatus NewStatus, string PriorityCodeId, string? AdminRemakrs) : IRequest<string>;

public class UpdateComplaintStatusCommandHandler : IRequestHandler<UpdateComplaintStatusCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateComplaintStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(UpdateComplaintStatusCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(currentUserId))
            throw new UnAuthorizedException("Invalid user context.");

        var complaint = await _context.Complaints
            .FirstOrDefaultAsync(x => x.Id == request.ComplaintId, cancellationToken)
            ?? throw new ArgumentException("Complaint not found.");

        var oldStatus = complaint.Status;
        complaint.Status = request.NewStatus;
        complaint.AdminRemarks = request.AdminRemakrs;
        complaint.PriorityCode = request.PriorityCodeId;
        complaint.LastModified = DateTime.Now;

        switch(request.NewStatus)
        {
            case ComplaintStatus.Acknowledged:
                complaint.AcknowledgedAt = DateTimeOffset.Now;
                break;

            case ComplaintStatus.Resolved:
                complaint.ResolvedAt = DateTimeOffset.Now;
                break;

            case ComplaintStatus.Closed:
                complaint.ClosedAt = DateTimeOffset.Now;
                break;

            default:
                // Do nothing for other statuses
                break;
        }

        // Add history record
        _context.ComplaintHistories.Add(new ComplaintHistory
        {
            ComplaintId = complaint.Id,
            Action = $"Status Updated from {oldStatus} to {request.NewStatus}",
            FromValue = oldStatus.ToString(),
            ToValue = request.NewStatus.ToString(),
            ActorUserId = currentUserId,
            AdminRemarks = request.AdminRemakrs,
            Created = DateTime.Now
        });

        await _context.SaveChangesAsync(cancellationToken);

        return $"Complaint status updated from {oldStatus} to {request.NewStatus}.";
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaintStatusCount;
public record GetMyComplaintStatusCountQuery() : IRequest<ComplaintStatusCountVm>;

public class GetMyComplaintStatusCountQueryHandler(
    IApplicationDbContext _context,
    ICurrentUserService _currentUser
) : IRequestHandler<GetMyComplaintStatusCountQuery, ComplaintStatusCountVm>
{
    public async Task<ComplaintStatusCountVm> Handle(GetMyComplaintStatusCountQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId.ToString()
            ?? throw new UnauthorizedAccessException("User not found");

        var statusGroups = await _context.Complaints
            .AsNoTracking()
            .Where(c => c.IsDeleted != true && c.CreatedByUserId == userId)
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var vm = new ComplaintStatusCountVm
        {
            TotalComplaints = statusGroups.Sum(x => x.Count),
            New = statusGroups.FirstOrDefault(x => x.Status == ComplaintStatus.New)?.Count ?? 0,
            Acknowledged = statusGroups.FirstOrDefault(x => x.Status == ComplaintStatus.Acknowledged)?.Count ?? 0,
            InProgress = statusGroups.FirstOrDefault(x => x.Status == ComplaintStatus.InProgress)?.Count ?? 0,
            Resolved = statusGroups.FirstOrDefault(x => x.Status == ComplaintStatus.Resolved)?.Count ?? 0,
            Closed = statusGroups.FirstOrDefault(x => x.Status == ComplaintStatus.Closed)?.Count ?? 0,
            Reopened = statusGroups.FirstOrDefault(x => x.Status == ComplaintStatus.Reopened)?.Count ?? 0
        };

        return vm;
    }
}


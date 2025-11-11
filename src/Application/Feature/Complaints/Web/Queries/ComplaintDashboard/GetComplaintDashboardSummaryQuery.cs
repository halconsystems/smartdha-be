using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.ComplaintDashboard;
public record GetComplaintDashboardSummaryQuery() : IRequest<ComplaintDashboardSummaryDto>;

public class GetComplaintDashboardSummaryQueryHandler : IRequestHandler<GetComplaintDashboardSummaryQuery, ComplaintDashboardSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetComplaintDashboardSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ComplaintDashboardSummaryDto> Handle(GetComplaintDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var complaints = _context.Complaints
            .Include(c => c.Attachments)
            .Include(c => c.Comments)
            .AsQueryable();

        var totalComplaints = await complaints.CountAsync(cancellationToken);

        // Complaints by status
        var complaintsByStatus = await complaints
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

        // Complaints by category
        var complaintsByCategory = await complaints
            .Join(_context.ComplaintCategories,
                  c => c.CategoryCode,       // complaint.CategoryCode
                  cat => cat.Id.ToString(),           // category.Code
                  (c, cat) => cat.Code)      // select Code
            .GroupBy(code => code)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count, cancellationToken);

        // Complaints by priority
        var complaintsByPriority = await complaints
            .Join(_context.ComplaintPriorities,
                c => c.PriorityCode,
                p => p.Id.ToString(),
                (c, p) => p.Code)
            .GroupBy(code => code)
            .Select(g => new { Priority = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Priority, x => x.Count, cancellationToken);

        // Recent 5 complaints
        var recentComplaints = await complaints
            .OrderByDescending(c => c.Created)
            .Take(5)
            .Select(c => new RecentComplaintDto
            {
                Id = c.Id,
                ComplaintNo = c.ComplaintNo,
                Title = c.Title,
                Status = c.Status.ToString(),
                Category = c.CategoryCode,
                Priority = c.PriorityCode,
                Created = c.Created
            })
            .ToListAsync(cancellationToken);

        return new ComplaintDashboardSummaryDto
        {
            TotalComplaints = totalComplaints,
            ComplaintsByStatus = complaintsByStatus,
            ComplaintsByCategory = complaintsByCategory,
            ComplaintsByPriority = complaintsByPriority,
            RecentComplaints = recentComplaints
        };
    }
}

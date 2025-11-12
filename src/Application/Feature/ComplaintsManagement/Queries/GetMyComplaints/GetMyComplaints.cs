using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaints;
public record GetMyComplaintsQuery() : IRequest<List<MyComplaintVm>>;

public class GetMyComplaintsQueryHandler(
    IApplicationDbContext _context,
    ICurrentUserService _currentUser
) : IRequestHandler<GetMyComplaintsQuery, List<MyComplaintVm>>
{
    public async Task<List<MyComplaintVm>> Handle(GetMyComplaintsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId.ToString() ?? throw new UnauthorizedAccessException("User not found");

        var complaints = await _context.Complaints
            .AsNoTracking()
            .Where(c => c.CreatedByUserId == userId && c.IsDeleted != true)
            .OrderByDescending(c => c.Created)
            .Select(c => new MyComplaintVm
            {
                Id = c.Id,
                ComplaintNo = c.ComplaintNo,
                Title = c.Title,
                Notes = c.Notes,
                CategoryCode = c.CategoryCode,
                PriorityCode = c.PriorityCode,
                Status = c.Status.ToString(), // Enum -> string (e.g., "New", "Resolved")
                Created = c.Created,
                ImageUrls = c.Attachments
                    .Where(a => a.IsDeleted != true)
                    .Select(a => a.ImageURL)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return complaints;
    }
}


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
    ICurrentUserService _currentUser,
    IFileStorageService _fileStorageService
) : IRequestHandler<GetMyComplaintsQuery, List<MyComplaintVm>>
{
    public async Task<List<MyComplaintVm>> Handle(GetMyComplaintsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId.ToString() ?? throw new UnauthorizedAccessException("User not found");

        // STEP 1: Fetch from database
        var complaints = await _context.Complaints
            .Include(c => c.Attachments)
            .AsNoTracking()
            .Where(c => c.CreatedByUserId == userId && c.IsDeleted != true)
            .OrderByDescending(c => c.Created)
            .ToListAsync(cancellationToken);

        var categoryColors = await _context.ComplaintCategories
        .AsNoTracking()
        .Where(c => c.IsActive==true)
        .ToDictionaryAsync(c => c.Code, c => c.ColorCode ?? "#000000", cancellationToken);

        // STEP 2: Map in memory
        var result = complaints.Select(c => new MyComplaintVm
        {
            Id = c.Id,
            ComplaintNo = c.ComplaintNo,
            Title = c.Title,
            Notes = c.Notes,
            CategoryCode = c.CategoryCode,
            PriorityCode = c.PriorityCode,
            ColorCode = categoryColors.TryGetValue(c.CategoryCode, out var color)
            ? color
            : "#000000",  // fallback color
            Status = c.Status.ToString(),
            Created = c.Created,
            ImageUrls = c.Attachments
                .Where(a => a.IsDeleted != true && !string.IsNullOrWhiteSpace(a.ImageURL))
                .Select(a => _fileStorageService.GetPublicUrlOfComplaint(a.ImageURL))
                .ToList()
        }).ToList();

        return result;
    }
}

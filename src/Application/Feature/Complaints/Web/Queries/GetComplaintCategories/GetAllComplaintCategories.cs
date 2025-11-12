using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetComplaintCategories;
public record GetAllComplaintCategoriesQuery() : IRequest<SuccessResponse<List<ComplaintCategory>>>;

public class GetAllComplaintCategoriesQueryHandler
    : IRequestHandler<GetAllComplaintCategoriesQuery, SuccessResponse<List<ComplaintCategory>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllComplaintCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<ComplaintCategory>>> Handle(GetAllComplaintCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.ComplaintCategories
            .Where(c => c.IsActive == true && c.IsDeleted == false)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return Success.Ok(categories);
    }
}

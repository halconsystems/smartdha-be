using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetComplaintPriorities;

public record GetAllComplaintPrioritiesQuery() : IRequest<SuccessResponse<List<ComplaintPriority>>>;

public class GetAllComplaintPrioritiesQueryHandler
    : IRequestHandler<GetAllComplaintPrioritiesQuery, SuccessResponse<List<ComplaintPriority>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllComplaintPrioritiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<ComplaintPriority>>> Handle(GetAllComplaintPrioritiesQuery request, CancellationToken cancellationToken)
    {
        var priorities = await _context.ComplaintPriorities
            .Where(p => p.IsActive == true && p.IsDeleted == false)
            .OrderBy(p => p.Weight)
            .ToListAsync(cancellationToken);

        return Success.Ok(priorities);
    }
}

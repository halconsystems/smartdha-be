using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetComplaintDropdowns;
public record GetComplaintDropdownsQuery : IRequest<ComplaintDropdownVm>;

public class GetComplaintDropdownsQueryHandler(IApplicationDbContext _context)
    : IRequestHandler<GetComplaintDropdownsQuery, ComplaintDropdownVm>
{
    public async Task<ComplaintDropdownVm> Handle(GetComplaintDropdownsQuery request, CancellationToken ct)
    {
        var categories = await _context.ComplaintCategories
            .Where(c => c.IsActive == true && c.IsDeleted==false)
            .OrderBy(c => c.Name)
            .Select(c => new DropdownVm(
                c.Id.ToString(),         // primary key
                c.Code,       // short code like "WATER"
                c.Name        // display text
            ))
            .ToListAsync(ct);

        var priorities = await _context.ComplaintPriorities
            .Where(p => p.IsActive == true && p.IsDeleted == false)
            .OrderBy(p => p.Weight)
            .Select(p => new DropdownVm(
                p.Id.ToString(),
                p.Code,
                p.Name
            ))
            .ToListAsync(ct);

        return new ComplaintDropdownVm(categories, priorities);
    }
}


public record DropdownVm(
    string Id,          // Database primary key
    string Code,     // e.g. "WATER"
    string Name      // e.g. "Water Supply"
);

public record ComplaintDropdownVm(
    List<DropdownVm> Categories,
    List<DropdownVm> Priorities
);



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
public record SubModuleListQuery : IRequest<SuccessResponse<List<SubModulesDto>>>
{ public Guid? Id { get; set; } };

public class SubModuleListQueryHandler : IRequestHandler<SubModuleListQuery, SuccessResponse<List<SubModulesDto>>>
{
    private readonly IApplicationDbContext _context;

    public SubModuleListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<SuccessResponse<List<SubModulesDto>>> Handle(SubModuleListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SubModules.AsQueryable();

        if (request.Id.HasValue)
        {
            query = query.Where(s => s.Id == request.Id);
        }

        var subModules = await query
            .Select(s => new SubModulesDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ModuleId = s.ModuleId
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<SubModulesDto>>(subModules, "Sub-modules fetched successfully.", "SubModules");
    }



}

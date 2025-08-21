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
public record GetSubModuleListQuery : IRequest<SuccessResponse<List<SubModulesDto>>>
{ public Guid? Id { get; set; } };

public class GetSubModuleListQueryHandler : IRequestHandler<GetSubModuleListQuery, SuccessResponse<List<SubModulesDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSubModuleListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<SuccessResponse<List<SubModulesDto>>> Handle(GetSubModuleListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SubModules.Where(x => x.IsDeleted == null || x.IsDeleted == false).AsQueryable();

        if (request.Id.HasValue)
        {
            query = query.Where(s => s.Id == request.Id);
        }

        var subModules = await query
            .Select(s => new SubModulesDto
            {
                Id = s.Id,
                Value=s.Value,
                DisplayName=s.DisplayName,
                Name = s.Name,
                Description = s.Description,
                ModuleId = s.ModuleId,
                RequiresPermission=s.RequiresPermission

            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<SubModulesDto>>(subModules, "Sub-modules fetched successfully.", "SubModules");
    }



}

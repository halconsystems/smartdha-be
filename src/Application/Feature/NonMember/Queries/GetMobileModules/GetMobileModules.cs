using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetMobileModules;
public class GetMobileModulesQuery : IRequest<SuccessResponse<List<ModuleDto>>>
{
}
public class GetMobileModulesQueryHandler : IRequestHandler<GetMobileModulesQuery, SuccessResponse<List<ModuleDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMobileModulesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<ModuleDto>>> Handle(GetMobileModulesQuery request, CancellationToken cancellationToken)
    {
        var mobileModules = await _context.Modules
            .Where(m => m.AppType == AppType.Mobile && (m.IsDeleted == false || m.IsDeleted == null))
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Title = m.Title,
                Remarks = m.Remarks
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<ModuleDto>>(mobileModules);
    }
}


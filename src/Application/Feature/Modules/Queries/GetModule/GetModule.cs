using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetModule;
public class GetModulesQuery : IRequest<SuccessResponse<List<ModuleDto>>>
{
    public string? Id { get; set; }  // Accept from user
}

public class GetModulesQueryHandler : IRequestHandler<GetModulesQuery, SuccessResponse<List<ModuleDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetModulesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<ModuleDto>>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Modules.AsQueryable();

        if (Guid.TryParse(request.Id, out Guid parsedId) && parsedId != Guid.Empty)
        {
            query = query.Where(x => x.Id == parsedId);
        }

        var modules = await query
            .Select(x => new ModuleDto
            {
                Id = x.Id,
                Value = x.Value,
                DisplayName = x.DisplayName,
                Name = x.Name,
                Description = x.Description,
                Title = x.Title,
                Remarks = x.Remarks,
                AppType = x.AppType
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<ModuleDto>>(modules);
    }

}



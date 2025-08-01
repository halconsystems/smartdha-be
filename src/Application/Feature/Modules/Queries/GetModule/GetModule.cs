using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetModule;
public class GetModulesQuery : IRequest<List<ModuleDto>>
{
    public string? Id { get; set; }  // Accept from user
}

public class GetModulesQueryHandler : IRequestHandler<GetModulesQuery, List<ModuleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetModulesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Modules.AsQueryable();

        if (Guid.TryParse(request.Id, out Guid parsedId) && parsedId != Guid.Empty)
        {
            query = query.Where(x => x.Id == parsedId);
        }

        return await query
            .Select(x => new ModuleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Title = x.Title,
                Remarks = x.Remarks,
                AppType = x.AppType
            })
            .ToListAsync(cancellationToken);
    }
}



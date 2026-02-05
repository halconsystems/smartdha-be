using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public record GetServiceDefinitionsDropdownQuery
    : IRequest<List<DropdownDto>>;
public class GetServiceDefinitionsDropdownHandler
    : IRequestHandler<GetServiceDefinitionsDropdownQuery, List<DropdownDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetServiceDefinitionsDropdownHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<List<DropdownDto>> Handle(
        GetServiceDefinitionsDropdownQuery request,
        CancellationToken ct)
    {
        return await _db.ServiceDefinitions
            .AsNoTracking()
            .Where(x => x.IsActive == true && x.IsDeleted != true)
            .OrderBy(x => x.Name)
            .Select(x => new DropdownDto(x.Id, x.Name))
            .ToListAsync(ct);
    }
}


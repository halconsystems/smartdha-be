using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public record GetAllFacilitiesDropdownQuery()
    : IRequest<List<DropdownDto>>;
public class GetAllFacilitiesDropdownHandler
    : IRequestHandler<GetAllFacilitiesDropdownQuery, List<DropdownDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetAllFacilitiesDropdownHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<List<DropdownDto>> Handle(
        GetAllFacilitiesDropdownQuery request,
        CancellationToken ct)
    {
        return await _db.Facilities
            .AsNoTracking()
            .Where(x => x.IsDeleted != true && x.IsActive == true)
            .OrderBy(x => x.DisplayName)
            .Select(x => new DropdownDto(
                x.Id,
                x.DisplayName
            ))
            .ToListAsync(ct);
    }
}


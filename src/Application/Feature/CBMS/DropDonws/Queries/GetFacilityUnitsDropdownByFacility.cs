using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public record GetFacilityUnitsDropdownByFacilityQuery(Guid FacilityId)
    : IRequest<List<DropdownDto>>;
public class GetFacilityUnitsDropdownByFacilityHandler
    : IRequestHandler<GetFacilityUnitsDropdownByFacilityQuery, List<DropdownDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetFacilityUnitsDropdownByFacilityHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<List<DropdownDto>> Handle(
        GetFacilityUnitsDropdownByFacilityQuery request,
        CancellationToken ct)
    {
        return await _db.FacilityUnits
            .AsNoTracking()
            .Where(x =>
                x.FacilityId == request.FacilityId &&
                x.IsDeleted != true &&
                x.IsActive == true)
            .OrderBy(x => x.Name)
            .Select(x => new DropdownDto(x.Id, x.Name))
            .ToListAsync(ct);
    }
}


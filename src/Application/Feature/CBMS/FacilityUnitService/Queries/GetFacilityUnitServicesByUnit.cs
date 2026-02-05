using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Queries;



public record GetFacilityUnitServicesByUnitQuery(Guid FacilityUnitId)
    : IRequest<List<FacilityUnitServiceDto>>;


public class GetFacilityUnitServicesByUnitHandler
    : IRequestHandler<GetFacilityUnitServicesByUnitQuery, List<FacilityUnitServiceDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetFacilityUnitServicesByUnitHandler(
        ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<FacilityUnitServiceDto>> Handle(
        GetFacilityUnitServicesByUnitQuery request,
        CancellationToken ct)
    {
        // ✅ Validate input
        if (request.FacilityUnitId == Guid.Empty)
            throw new ArgumentException("FacilityUnitId is required");

        // ✅ Query services for specific facility unit
        var services = await _db.FacilityUnitServices
            .AsNoTracking()
            .Include(x => x.ServiceDefinition)
            .Where(x =>
                x.FacilityUnitId == request.FacilityUnitId &&
                x.IsDeleted != true &&
                x.IsEnabled == true
            )
            .OrderBy(x => x.ServiceDefinition.Name)
            .Select(x => new FacilityUnitServiceDto(
                x.Id,
                x.FacilityUnitId,
                x.ServiceDefinitionId,
                x.ServiceDefinition.Name,
                x.Price,
                x.IsComplimentary,
                x.IsEnabled
            )).ToListAsync(ct);

        return services;
    }
}






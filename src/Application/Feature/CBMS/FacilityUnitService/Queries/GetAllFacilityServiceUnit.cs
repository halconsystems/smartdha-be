using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Queries;
public record FacilityUnitServiceDto(
    Guid Id,
    Guid FacilityUnitId,
    Guid ServiceDefinitionId,
    string ServiceName,
    decimal Price,
    bool IsComplimentary,
    bool IsEnabled
);
public record GetAllFacilityUnitServicesQuery : IRequest<List<FacilityUnitServiceDto>>;
public class GetAllFacilityUnitServicesHandler
    : IRequestHandler<GetAllFacilityUnitServicesQuery, List<FacilityUnitServiceDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetAllFacilityUnitServicesHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<List<FacilityUnitServiceDto>> Handle(
        GetAllFacilityUnitServicesQuery request,
        CancellationToken ct)
    {
        return await _db.FacilityUnitServices
            .Include(x => x.ServiceDefinition)
            .Where(x => x.IsDeleted != true)
            .Select(x => new FacilityUnitServiceDto(
                x.Id,
                x.FacilityUnitId,
                x.ServiceDefinitionId,
                x.ServiceDefinition.Name,
                x.Price,
                x.IsComplimentary,
                x.IsEnabled
            ))
            .ToListAsync(ct);
    }
}






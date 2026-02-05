using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Queries.GetAllServiceDefinitions;
public record GetAllServiceDefinitionsQuery
    : IRequest<ApiResult<List<ServiceDefinitionDto>>>;
public class GetAllServiceDefinitionsQueryHandler
    : IRequestHandler<GetAllServiceDefinitionsQuery, ApiResult<List<ServiceDefinitionDto>>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetAllServiceDefinitionsQueryHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<List<ServiceDefinitionDto>>> Handle(
        GetAllServiceDefinitionsQuery request,
        CancellationToken ct)
    {
        var list = await _db.ServiceDefinitions
            .AsNoTracking()
            .Where(x => x.IsDeleted != true)
            .OrderBy(x => x.Name)
            .Select(x => new ServiceDefinitionDto(
                x.Id,
                x.ClubServiceCategoryId,
                x.ClubServiceCategory.Name,
                x.Name,
                x.Code,
                x.IsQuantityBased,
                x.IsActive
            ))
            .ToListAsync(ct);

        return ApiResult<List<ServiceDefinitionDto>>.Ok(list);
    }
}


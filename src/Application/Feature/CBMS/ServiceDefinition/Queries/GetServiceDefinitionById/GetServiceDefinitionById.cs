using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Queries.GetServiceDefinitionById;
public record GetServiceDefinitionByIdQuery(Guid Id)
    : IRequest<ApiResult<ServiceDefinitionDto>>;
public class GetServiceDefinitionByIdQueryHandler
    : IRequestHandler<GetServiceDefinitionByIdQuery, ApiResult<ServiceDefinitionDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetServiceDefinitionByIdQueryHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<ServiceDefinitionDto>> Handle(
        GetServiceDefinitionByIdQuery request,
        CancellationToken ct)
    {
        var item = await _db.ServiceDefinitions
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.IsDeleted != true)
            .Select(x => new ServiceDefinitionDto(
                x.Id,
                x.ClubServiceCategoryId,
                x.ClubServiceCategory.Name,
                x.Name,
                x.Code,
                x.IsQuantityBased,
                x.IsActive
            ))
            .FirstOrDefaultAsync(ct);

        if (item == null)
            return ApiResult<ServiceDefinitionDto>.Fail("Service not found.");

        return ApiResult<ServiceDefinitionDto>.Ok(item);
    }
}


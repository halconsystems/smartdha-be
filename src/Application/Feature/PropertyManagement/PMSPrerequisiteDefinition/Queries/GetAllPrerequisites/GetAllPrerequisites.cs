using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetAllPrerequisites;
public record PrerequisiteDefinitionDto
(
    Guid Id,
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions
);
public record GetAllPrerequisitesQuery()
    : IRequest<ApiResult<List<PrerequisiteDefinitionDto>>>;
public class GetAllPrerequisitesHandler
    : IRequestHandler<GetAllPrerequisitesQuery, ApiResult<List<PrerequisiteDefinitionDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetAllPrerequisitesHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<PrerequisiteDefinitionDto>>> Handle(
        GetAllPrerequisitesQuery request,
        CancellationToken ct)
    {
        var list = await _db.Set<PrerequisiteDefinition>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new PrerequisiteDefinitionDto(
                x.Id,
                x.Name,
                x.Code,
                x.Type,
                x.MinLength,
                x.MaxLength,
                x.AllowedExtensions
            ))
            .ToListAsync(ct);

        return ApiResult<List<PrerequisiteDefinitionDto>>.Ok(list);
    }
}


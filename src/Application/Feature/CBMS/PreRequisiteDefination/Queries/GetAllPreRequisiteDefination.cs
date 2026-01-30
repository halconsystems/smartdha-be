using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisiteDefination.Queries;


public record PrerequisiteDefinitionDto
(
    Guid Id,
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    List<PrerequisiteOptionDto> Options
);
public record GetAllPreRequisiteDefinationQuery()
    : IRequest<ApiResult<List<PrerequisiteDefinitionDto>>>;
public class GetAllPreRequisiteDefinationQueryHandler
    : IRequestHandler<GetAllPreRequisiteDefinationQuery, ApiResult<List<PrerequisiteDefinitionDto>>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public GetAllPreRequisiteDefinationQueryHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<PrerequisiteDefinitionDto>>> Handle(
        GetAllPreRequisiteDefinationQuery request,
        CancellationToken ct)
    {
        var list = await _db.Set<ClubPrerequisiteDefinitions>()
            .Where(x => x.IsActive == true && x.IsDeleted == false)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new PrerequisiteDefinitionDto(
                x.Id,
                x.Name,
                x.Code,
                x.Type,
                x.MinLength,
                x.MaxLength,
                x.AllowedExtensions,

                // ✅ Load options only if applicable
                x.Type == PrerequisiteType.Dropdown ||
                x.Type == PrerequisiteType.MultiSelect ||
                x.Type == PrerequisiteType.CheckboxGroup ||
                x.Type == PrerequisiteType.RadioGroup
                    ? _db.Set<ClubPrerequisiteOptions>()
                        .Where(o => o.PrerequisiteDefinitionId == x.Id && o.IsDeleted == false)
                        .OrderBy(o => o.SortOrder)
                        .Select(o => new PrerequisiteOptionDto(
                            o.Id,
                            o.Label,
                            o.Value,
                            o.SortOrder
                        ))
                        .ToList()
                    : new List<PrerequisiteOptionDto>()
            ))
            .ToListAsync(ct);

        return ApiResult<List<PrerequisiteDefinitionDto>>.Ok(list);
    }
}


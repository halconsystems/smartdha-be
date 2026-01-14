using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;

public record ProcessPrerequisiteDto
(
    Guid PrerequisiteDefinitionId,
    string Code,
    string Name,
    PrerequisiteType Type,

    bool IsRequired,
    int RequiredByStepNo,

    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions
);

public record GetProcessPrerequisiteQuery(Guid ProcessId) : IRequest<ApiResult<List<ProcessPrerequisiteDto>>>;

public class GetProcessPrerequisitesHandler : IRequestHandler<GetProcessPrerequisiteQuery, ApiResult<List<ProcessPrerequisiteDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetProcessPrerequisitesHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<ProcessPrerequisiteDto>>> Handle(
        GetProcessPrerequisiteQuery request,
        CancellationToken ct)
    {
        var processExists = await _db.Set<ServiceProcess>()
            .AnyAsync(x => x.Id == request.ProcessId, ct);

        if (!processExists)
            return ApiResult<List<ProcessPrerequisiteDto>>
                .Fail("Process not found.");

        var result = await _db.Set<ProcessPrerequisite>()
            .Where(x => x.ProcessId == request.ProcessId)
            .OrderBy(x => x.RequiredByStepNo)
            .Select(x => new ProcessPrerequisiteDto(
                x.PrerequisiteDefinitionId,
                x.PrerequisiteDefinition.Code,
                x.PrerequisiteDefinition.Name,
                x.PrerequisiteDefinition.Type,
                x.IsRequired,
                x.RequiredByStepNo,
                x.PrerequisiteDefinition.MinLength,
                x.PrerequisiteDefinition.MaxLength,
                x.PrerequisiteDefinition.AllowedExtensions
            ))
            .ToListAsync(ct);

        return ApiResult<List<ProcessPrerequisiteDto>>.Ok(result);
    }
}


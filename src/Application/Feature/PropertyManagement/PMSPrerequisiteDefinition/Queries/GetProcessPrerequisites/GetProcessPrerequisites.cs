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
public record UserPropertyDto(
    Guid PropertyId,
    string PropertyNo,
    string? Sector,
    string? Phase
);

public record ProcessPrerequisiteResponseDto
(
    List<UserPropertyDto> Properties,
    bool IsNadra,
    bool IsFeeRequired,      
    bool IsFeeAtSubmission,  
    bool IsVoucherPossible,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    string? Instruction,
    List<ProcessPrerequisiteDto> Prerequisites
);

public record GetProcessPrerequisiteQuery(Guid ProcessId)
    : IRequest<ApiResult<ProcessPrerequisiteResponseDto>>;

public class GetProcessPrerequisitesHandler
    : IRequestHandler<GetProcessPrerequisiteQuery, ApiResult<ProcessPrerequisiteResponseDto>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IUser _currentUser;

    public GetProcessPrerequisitesHandler(
        IPMSApplicationDbContext db,
        IUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<ProcessPrerequisiteResponseDto>> Handle(
        GetProcessPrerequisiteQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_currentUser.Id))
            return ApiResult<ProcessPrerequisiteResponseDto>
                .Fail("User not authenticated.");

        // 1️⃣ Validate process
        var process = await _db.Set<ServiceProcess>()
    .Where(x => x.Id == request.ProcessId)
    .Select(x => new
    {
        x.IsFeeRequired,
        x.IsFeeAtSubmission,
        x.IsVoucherPossible,
        x.IsfeeSubmit,
        x.IsInstructionAtStart,
        x.IsNadraVerificationRequired,
        x.Instruction
    })
    .FirstOrDefaultAsync(ct);

        if (process == null)
            return ApiResult<ProcessPrerequisiteResponseDto>.Fail("Process not found.");

        // 2️⃣ Load USER PROPERTIES
        var properties = await _db.Set<UserProperty>()
            .AsNoTracking()
            .Where(x => x.CreatedBy == _currentUser.Id)
            .Select(x => new UserPropertyDto(
                x.Id,
                x.PropertyNo,
                x.Sector,
                x.Phase
            ))
            .ToListAsync(ct);

        // 3️⃣ Load prerequisites
        var prerequisites = await _db.Set<ProcessPrerequisite>()
            .AsNoTracking()
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

        

        // 6️⃣ Final response
        var response = new ProcessPrerequisiteResponseDto(
            properties,
            process.IsNadraVerificationRequired,
            process.IsFeeRequired,
            process.IsFeeAtSubmission,
            process.IsVoucherPossible,
            process.IsfeeSubmit,
            process.IsInstructionAtStart,
            process.Instruction,
            prerequisites
        );

        return ApiResult<ProcessPrerequisiteResponseDto>.Ok(response);
    }
}

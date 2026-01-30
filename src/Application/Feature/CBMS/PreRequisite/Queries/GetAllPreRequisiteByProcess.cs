using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisite.Queries;

public record ClubProcessPrerequisiteResponseDto
(
    List<UserClubDto> ClubDtos,
    bool IsFeeRequired,
    bool IsFeeAtSubmission,
    bool IsVoucherPossible,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    string? Instruction,
    List<ProcessPrerequisiteDto> Prerequisites
);
public record UserClubDto(
    Guid ClubId,
    string ClubName,
    string? Location,
    string? PhoneNumber
);
public record GetClubProcessAllPrerequisiteQuery(Guid ProcessId)
    : IRequest<ApiResult<ClubProcessPrerequisiteResponseDto>>;

public class GetProcessAllPrerequisitesHandler
    : IRequestHandler<GetClubProcessAllPrerequisiteQuery, ApiResult<ClubProcessPrerequisiteResponseDto>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IUser _currentUser;

    public GetProcessAllPrerequisitesHandler(
        IPMSApplicationDbContext db,
        IUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<ClubProcessPrerequisiteResponseDto>> Handle(
        GetClubProcessAllPrerequisiteQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_currentUser.Id))
            return ApiResult<ClubProcessPrerequisiteResponseDto>
                .Fail("User not authenticated.");

        // 1️⃣ Validate process
        // 1️⃣ Validate process
        var process = await _db.Set<ClubServiceProcess>()
       .Where(x => x.Id == request.ProcessId && x.IsActive == true && x.IsDeleted == false)
       .Select(x => new
       {
           x.IsFeeRequired,
           x.IsFeeAtSubmission,
           x.IsVoucherPossible,
           x.IsfeeSubmit,
           x.IsInstructionAtStart,
           x.Instruction
       })
       .FirstOrDefaultAsync(ct);

        if (process == null)
            return ApiResult<ClubProcessPrerequisiteResponseDto>.Fail("Process not found.");


        // 2️⃣ Load USER PROPERTIES
        var properties = await _db.Set<Club>()
            .Where(x => x.IsActive == true && x.IsDeleted == false)
            .AsNoTracking()
            //.Where(x => x.CreatedBy == _currentUser.Id)
            .Select(x => new UserClubDto(
                x.Id,
                x.Name,
                x.Location,
                x.ContactNumber
            ))
            .ToListAsync(ct);

        // 3️⃣ Load prerequisites
        var prerequisites = await _db.Set<ClubProcessPrerequisite>().Where(x => x.IsActive == true && x.IsDeleted == false)
            .AsNoTracking()
            .Where(x => x.ProcessId == request.ProcessId)
            .OrderBy(x => x.RequiredByStepNo)
            .Select(x => new ProcessPrerequisiteDto(
                x.Id,
                x.ProcessId,
                x.PrerequisiteDefinitionId,
                x.PrerequisiteDefinition.Code,
                x.PrerequisiteDefinition.Name,
                x.PrerequisiteDefinition.Type,
                x.IsRequired,
                x.RequiredByStepNo,
                x.PrerequisiteDefinition.MinLength,
                x.PrerequisiteDefinition.MaxLength,
                x.PrerequisiteDefinition.AllowedExtensions,
                x.PrerequisiteDefinition.Type == PrerequisiteType.Dropdown ||
                x.PrerequisiteDefinition.Type == PrerequisiteType.MultiSelect ||
                x.PrerequisiteDefinition.Type == PrerequisiteType.CheckboxGroup ||
                x.PrerequisiteDefinition.Type == PrerequisiteType.RadioGroup
                    ? _db.Set<PrerequisiteOption>()
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



        //  Final response
        var response = new ClubProcessPrerequisiteResponseDto(
            properties,
            process.IsFeeRequired,
            process.IsFeeAtSubmission,
            process.IsVoucherPossible,
            process.IsfeeSubmit,
            process.IsInstructionAtStart,
            process.Instruction,
            prerequisites
        );

        return ApiResult<ClubProcessPrerequisiteResponseDto>.Ok(response);
    }
}



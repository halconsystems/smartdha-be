using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;

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
    private readonly ICurrentUserService _currentUserService;
    private readonly IProcedureService _procedureService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPropertyProcedureRepository _repo;


    public GetProcessPrerequisitesHandler(
        IPMSApplicationDbContext db,
        ICurrentUserService currentUserService,
        IProcedureService procedureService,
        UserManager<ApplicationUser> userManager,
    IPropertyProcedureRepository repo
)
    {
        _db = db;
        _currentUserService = currentUserService;
        _procedureService = procedureService;
        _userManager = userManager;
        _repo = repo;
    }

    public async Task<ApiResult<ProcessPrerequisiteResponseDto>> Handle(
        GetProcessPrerequisiteQuery request,
        CancellationToken ct)
    {

        var userId = _currentUserService.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new UnAuthorizedException("Invalid CNIC. Please verify and try again.");
        }

        // 1️⃣ Get properties from Stored Procedure
        //var spProperties =
        //    await _repo.GetPropertiesByCnicAsync(user.CNIC, ct);

        //if (!spProperties.Any())
        //    throw new Exception("No Propertties Found.");

        // 2️⃣ Deactivate existing properties
        //var existing = await _db.Properties
        //    .Where(x =>
        //        x.OwnerCnic == user.CNIC &&
        //        x.IsActive == true && x.IsDeleted != true)
        //    .ToListAsync(ct);

        //foreach (var prop in existing)
        //{
        //    prop.IsActive = false;
        //    prop.IsDeleted = true;
        //}

        // 3️⃣ Insert fresh properties
        //var newEntities = spProperties.Select(x => new UserProperty
        //{
        //    PropertyNo = x.PLOT_NO ?? x.PLTNO!,
        //    PlotNo = x.PLOT_NO ?? x.PLTNO!,
        //    Sector = x.STNAME ?? x.SUBDIV,
        //    Phase = x.PHASE,
        //    Area = x.ACTUAL_SIZE,
        //    OwnerCnic = x.NIC,
        //    PropertyPk = x.PLOTPK,
        //    MemberPk = x.MEMPK,
        //    MemberNo = x.MEMNO,
        //    CellNo = x.CELLNO,
        //    IsActive = true,
        //    IsDeleted = false
        //});

        //await _db.Properties.AddRangeAsync(newEntities, ct);
        //await _db.SaveChangesAsync(ct);

        // 1️⃣ Validate process
        var process = await _db.Set<ServiceProcess>()
    .Where(x => x.Id == request.ProcessId && x.IsActive==true && x.IsDeleted==false)
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
            .Where(x => x.OwnerCnic == user.CNIC && x.IsActive==true && x.IsDeleted==false)
            .OrderByDescending(x=> x.Created)
            .Select(x => new UserPropertyDto(
                x.Id,
                x.PlotNo ?? "",
                x.SubDivision,
                x.Phase
            ))
            .ToListAsync(ct);

        // 3️⃣ Load prerequisites
        var prerequisites = await _db.Set<ProcessPrerequisite>()
    .AsNoTracking()
    .Where(x => x.ProcessId == request.ProcessId && x.IsActive == true && x.IsDeleted == false)
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

        // ✅ Load options correctly
        x.PrerequisiteDefinition.Type == PrerequisiteType.Dropdown ||
        x.PrerequisiteDefinition.Type == PrerequisiteType.MultiSelect ||
        x.PrerequisiteDefinition.Type == PrerequisiteType.CheckboxGroup ||
        x.PrerequisiteDefinition.Type == PrerequisiteType.RadioGroup
            ? _db.Set<PrerequisiteOption>()
                .Where(o =>
                    o.PrerequisiteDefinitionId == x.PrerequisiteDefinitionId && x.IsActive==true &&
                    o.IsDeleted == false)
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

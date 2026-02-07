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
    string? PropertyId,
    string? PropertyNo,
    string? Sector,
    string? Phase,
    string? Address
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPropertyInfoService _propertyInfoService;
    public GetProcessPrerequisitesHandler(
        IPMSApplicationDbContext db,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager,
        IPropertyInfoService propertyInfoService
)
    {
        _db = db;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _propertyInfoService = propertyInfoService;
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

        var spProperties = await _propertyInfoService.GetPropertiesByCnicAsync(user.CNIC, ct);

        if (spProperties == null || !spProperties.Any())
        {
            throw new Exception("No property found.");
        }

        var properties = spProperties
            .Select(x => new UserPropertyDto(
                PropertyId: x.PLOTPK,              // No local DB Id yet
                PropertyNo: x.PLOT_NO ?? x.PLTNO ?? string.Empty,
                Sector: x.SUBDIV,
                Phase: x.PHASE,
                Address:x.PROPERTY_ADDRESS
            ))
            .ToList();

        // 2️⃣ Load USER PROPERTIES
        //var properties = await _db.Set<UserProperty>()
        //    .AsNoTracking()
        //    .Where(x => x.OwnerCnic == user.CNIC && x.IsActive==true && x.IsDeleted==false)
        //    .OrderByDescending(x=> x.Created)
        //    .Select(x => new UserPropertyDto(
        //        x.Id,
        //        x.PlotNo ?? "",
        //        x.SubDivision,
        //        x.Phase
        //    ))
        //    .ToListAsync(ct);

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

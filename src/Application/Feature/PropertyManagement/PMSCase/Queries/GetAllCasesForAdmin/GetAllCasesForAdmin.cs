using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetAllCasesForAdmin;

public class AdminCaseSummaryDto
{
    public Guid CaseId { get; set; }
    public string CaseNo { get; set; } = default!;

    public string ProcessName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;

    public CaseStatus Status { get; set; }
    public string StatusText { get; set; } = default!;

    public string? CurrentStepName { get; set; }
    public string? CurrentDirectorate { get; set; }

    public string ApplicantName { get; set; } = default!;
    public string ApplicantCnic { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    // 🔑 INTERNAL OWNERSHIP
    public string? AssignedUserId { get; set; }
    public bool IsAssignedToMe { get; set; }
    public string? AssignedUserDisplay { get; set; } // 👈 Ahmed (Clerk)

    public string CaseStatusText { get; set; } = default!;
    public string? CaseDirectorate { get; set; }
}


public record GetAllCasesForAdminQuery()
    : IRequest<ApiResult<List<AdminCaseSummaryDto>>>;
public class GetAllCasesForAdminHandler
    : IRequestHandler<GetAllCasesForAdminQuery, ApiResult<List<AdminCaseSummaryDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetAllCasesForAdminHandler(IPMSApplicationDbContext db, IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    {
        _db = db;
        _applicationDbContext = applicationDbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResult<List<AdminCaseSummaryDto>>> Handle(
        GetAllCasesForAdminQuery request,
        CancellationToken ct)
    {

        if (!_currentUserService.IsAuthenticated)
            return ApiResult<List<AdminCaseSummaryDto>>.Fail("Unauthorized.");

        var userId = _currentUserService.UserId!.ToString();

        // 1️⃣ Get modules of logged-in user
        var myModuleIds = await _applicationDbContext.Set<UserModuleAssignment>()
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.IsActive==true &&
                x.IsDeleted != true)
            .Select(x => x.ModuleId)
            .ToListAsync(ct);

        if (!myModuleIds.Any())
            return ApiResult<List<AdminCaseSummaryDto>>.Ok(new());


        var userRoleIds = await _applicationDbContext.AppUserRoles
    .Where(ur => ur.UserId == userId)
    .Select(ur => ur.RoleId)
    .ToListAsync(ct);

        var raw = await (
        from c in _db.Set<PropertyCase>().AsNoTracking()
        join ps in _db.Set<ProcessStep>().AsNoTracking()
            on c.ProcessId equals ps.ProcessId
        where
        c.IsActive ==true &&
        c.IsDeleted !=true &&
        myModuleIds.Contains(ps.Directorate.ModuleId) &&
        userRoleIds.Contains(ps.Directorate.RoleId)   // ✅ KEY FIX
        orderby c.Created descending
         select new
         {
             CaseId = c.Id,
             c.CaseNo,
             c.Status,
             c.CurrentStepNo,

             StepNo = ps.StepNo,
             StepName = ps.Name,
             DirectorateName = ps.Directorate.Name,



             ProcessName = c.Process.Name,
             CategoryName = c.Process.Category.Name,

             ApplicantName = c.ApplicantName,
             ApplicantCnic = c.ApplicantCnic,
             c.Created,

             AssignedUserId = c.CurrentAssignedUserId,
             CaseDirectorate= c.Directorate!.Name,
         }).ToListAsync(ct);





        var assignedUserIds = raw
        .Where(x => x.AssignedUserId != null)
        .Select(x => x.AssignedUserId!)
        .Distinct()
        .ToList();

        var users = await _applicationDbContext.Set<ApplicationUser>()
           .AsNoTracking()
           .Where(u => assignedUserIds.Contains(u.Id))
           .Select(u => new
           {
               u.Id,
               u.Name,
               Roles = u.UserRoles.Select(r => r.Role.Name)
           })
           .ToListAsync(ct);

               var userLookup = users.ToDictionary(
           u => u.Id,
           u =>
           {
               var roleText = u.Roles.Any()
                   ? string.Join(", ", u.Roles)
                   : "User";

               return $"{u.Name} ({roleText})";
           });


        var cases = raw.Select(x =>
        {
            var computedStatus = Compute(
                x.Status,
                x.CurrentStepNo,
                x.StepNo
             );

            string statusText = computedStatus switch
            {
                CaseStatus.Draft => "Draft",
                CaseStatus.Submitted => "Submitted",
                CaseStatus.InProgress => "In Progress",
                CaseStatus.Approved => "Approved",
                CaseStatus.Returned => "Returned",
                CaseStatus.Rejected => "Rejected",
                CaseStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };

           
            return new AdminCaseSummaryDto
            {
                CaseId = x.CaseId,
                CaseNo = x.CaseNo,
                ProcessName = x.ProcessName,
                CategoryName = x.CategoryName,

                Status = computedStatus,
                StatusText = statusText,

                CurrentStepName = x.StepName,
                CurrentDirectorate = x.DirectorateName,

                // 🔑 HERE IS THE ANSWER
                AssignedUserId = x.AssignedUserId,
                AssignedUserDisplay =
                    x.AssignedUserId != null &&
                    userLookup.TryGetValue(x.AssignedUserId, out var display)
                        ? display
                        : null,

                IsAssignedToMe =
                    x.AssignedUserId != null &&
                    x.AssignedUserId == userId,

                ApplicantName = x.ApplicantName ?? "-",
                ApplicantCnic = x.ApplicantCnic ?? "-",
                CreatedAt = x.Created,
                CaseDirectorate= x.CaseDirectorate,
                CaseStatusText = x.Status.ToString()
            };
        }).ToList();



        return ApiResult<List<AdminCaseSummaryDto>>.Ok(cases);
    }

    public static CaseStatus Compute(
    CaseStatus persistedStatus,
    int currentStepNo,
    int stepNo)
    {
        // 🔴 Absolute states
        if (persistedStatus == CaseStatus.Rejected)
            return CaseStatus.Rejected;

        if (persistedStatus == CaseStatus.Cancelled)
            return CaseStatus.Cancelled;

        if (persistedStatus == CaseStatus.Returned)
            return CaseStatus.Returned;

        if (persistedStatus == CaseStatus.Approved)
            return CaseStatus.Approved;

        // 🟡 Step-based states
        if (currentStepNo == stepNo)
            return CaseStatus.Submitted;

        if (currentStepNo < stepNo)
            return CaseStatus.Draft;

        if (currentStepNo > stepNo)
            return CaseStatus.InProgress;



        return CaseStatus.Approved;
    }

}

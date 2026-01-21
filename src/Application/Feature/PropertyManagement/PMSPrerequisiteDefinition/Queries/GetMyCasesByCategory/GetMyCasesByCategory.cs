using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetMyCasesByCategory;
public record GetMyCasesByCategoryQuery(Guid CategoryId)
    : IRequest<ApiResult<List<CaseListDto>>>;

public class GetMyCasesByCategoryHandler
    : IRequestHandler<GetMyCasesByCategoryQuery, ApiResult<List<CaseListDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    public GetMyCasesByCategoryHandler(
        IPMSApplicationDbContext db,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<ApiResult<List<CaseListDto>>> Handle(
        GetMyCasesByCategoryQuery request,
        CancellationToken ct)
    {
        var userId = _currentUserService.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new UnAuthorizedException("Invalid CNIC. Please verify and try again.");
        }

        // 1️⃣ Get user properties
        var userPropertyIds = await _db.Set<UserProperty>()
            .Where(x => x.OwnerCnic == user.CNIC && x.IsDeleted !=true)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (!userPropertyIds.Any())
            return ApiResult<List<CaseListDto>>.Ok(new());

        // 2️⃣ Get cases filtered by CategoryId
        var cases = await _db.Set<PropertyCase>()
            .AsNoTracking()
            .Where(x =>
                userPropertyIds.Contains(x.UserPropertyId) &&
                x.Process.CategoryId == request.CategoryId
            )
            .OrderByDescending(x => x.Created)
            .Select(x => new CaseListDto
            {
                CaseId = x.Id,
                CaseNo = x.CaseNo,
                Status = x.Status,

                ProcessName = x.Process.Name,
                CategoryName = x.Process.Category.Name,

                CurrentStepName = x.CurrentStep != null
                    ? x.CurrentStep.Name
                    : null,

                CurrentDirectorate = x.CurrentStep != null
                    ? x.CurrentStep.Directorate.Name
                    : null,

                CreatedAt = x.Created
            })
            .ToListAsync(ct);

        return ApiResult<List<CaseListDto>>.Ok(cases);
    }
}

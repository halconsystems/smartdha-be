using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetMyCasesByCategory;
public record GetMyCasesByCategoryQuery(Guid CategoryId)
    : IRequest<ApiResult<List<CaseListDto>>>;

public class GetMyCasesByCategoryHandler
    : IRequestHandler<GetMyCasesByCategoryQuery, ApiResult<List<CaseListDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IUser _currentUser;

    public GetMyCasesByCategoryHandler(
        IPMSApplicationDbContext db,
        IUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<List<CaseListDto>>> Handle(
        GetMyCasesByCategoryQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_currentUser.Id))
            return ApiResult<List<CaseListDto>>
                .Fail("User not authenticated.");

        // 1️⃣ Get user properties
        var userPropertyIds = await _db.Set<UserProperty>()
            .Where(x => x.CreatedBy == _currentUser.Id)
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

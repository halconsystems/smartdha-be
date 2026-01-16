using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;

public class CaseListDto
{
    public Guid CaseId { get; set; }
    public string CaseNo { get; set; } = default!;
    public CaseStatus Status { get; set; }

    public string ProcessName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;

    public string? CurrentStepName { get; set; }
    public string? CurrentDirectorate { get; set; }

    public DateTime CreatedAt { get; set; }
}


public record GetMyCasesQuery()
    : IRequest<ApiResult<List<CaseListDto>>>;

public class GetMyCasesHandler
    : IRequestHandler<GetMyCasesQuery, ApiResult<List<CaseListDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IUser _currentUser;

    public GetMyCasesHandler(
        IPMSApplicationDbContext db,
        IUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<List<CaseListDto>>> Handle(
        GetMyCasesQuery request,
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

        // 2️⃣ Get cases (SUMMARY ONLY)
        var cases = await _db.Set<PropertyCase>()
            .AsNoTracking()
            .Where(x => userPropertyIds.Contains(x.UserPropertyId))
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

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

public record MyCaseSummaryDto
(
    Guid CaseId,
    string CaseNo,
    string ProcessName,
    string CategoryName,

    CaseStatus Status,

    int? CurrentStepNo,
    string? CurrentStepName,
    string? CurrentDirectorate,

    DateTime CreatedDate
);

public record CaseApprovalHistoryDto
(
    int StepNo,
    string StepName,
    string DirectorateName,

    StepAction Action,
    string? Remarks,

    string? PerformedBy,
    DateTime ActionDate
);
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


public record MyCaseHistoryDto
(
    MyCaseSummaryDto CaseSummary,
    List<CaseApprovalHistoryDto> ApprovalHistory
);
public record GetMyCasesHistoryQuery()
    : IRequest<ApiResult<List<MyCaseHistoryDto>>>;

public class GetMyCasesHistoryHandler
    : IRequestHandler<GetMyCasesHistoryQuery, ApiResult<List<MyCaseHistoryDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IUser _currentUser;

    public GetMyCasesHistoryHandler(
        IPMSApplicationDbContext db,
        IUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<List<MyCaseHistoryDto>>> Handle(
        GetMyCasesHistoryQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_currentUser.Id))
            return ApiResult<List<MyCaseHistoryDto>>
                .Fail("User not authenticated.");

        // 1️⃣ Get user properties
        var userPropertyIds = await _db.Set<UserProperty>()
            .Where(x => x.CreatedBy == _currentUser.Id)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (userPropertyIds.Count == 0)
            return ApiResult<List<MyCaseHistoryDto>>.Ok(new());

        // 2️⃣ Get cases
        var cases = await _db.Set<PropertyCase>()
            .Include(x => x.Process)
                .ThenInclude(p => p.Category)
            .Include(x => x.CurrentStep)
            .Where(x => userPropertyIds.Contains(x.UserPropertyId))
            .OrderByDescending(x => x.Created)
            .ToListAsync(ct);

        var caseIds = cases.Select(x => x.Id).ToList();

        // 3️⃣ Load approval history
        var histories = await _db.Set<CaseStepHistory>()
            .Include(h => h.Step)
                .ThenInclude(s => s.Directorate)
            .Where(h => caseIds.Contains(h.CaseId))
            .OrderBy(h => h.Created)
            .ToListAsync(ct);

        // 4️⃣ Map result
        var result = cases.Select(c =>
        {
            var summary = new MyCaseSummaryDto(
                c.Id,
                c.CaseNo,
                c.Process.Name,
                c.Process.Category.Name,
                c.Status,
                c.CurrentStepNo,
                c.CurrentStep?.Name,
                c.CurrentStep?.Directorate?.Name,
                c.Created
            );

            var timeline = histories
                .Where(h => h.CaseId == c.Id)
                .Select(h => new CaseApprovalHistoryDto(
                    h.Step.StepNo,
                    h.Step.Name,
                    h.Step.Directorate.Name,
                    h.Action,
                    h.Remarks,
                    h.PerformedBy,
                    h.Created
                ))
                .ToList();

            return new MyCaseHistoryDto(summary, timeline);
        })
        .ToList();

        return ApiResult<List<MyCaseHistoryDto>>.Ok(result);
    }
}

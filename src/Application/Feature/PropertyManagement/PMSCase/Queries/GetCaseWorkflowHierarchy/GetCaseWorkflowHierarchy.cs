using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

public record CaseWorkflowStepDto
(
    int StepNo,
    string StepName,
    string DirectorateName,
    WorkflowStepStatus Status,
    DateTime? ActionDate,
    string? Remarks
);


public record GetCaseWorkflowHierarchyQuery(Guid CaseId)
    : IRequest<ApiResult<List<CaseWorkflowStepDto>>>;

public class GetCaseWorkflowHierarchyHandler
    : IRequestHandler<GetCaseWorkflowHierarchyQuery, ApiResult<List<CaseWorkflowStepDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetCaseWorkflowHierarchyHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<CaseWorkflowStepDto>>> Handle(
        GetCaseWorkflowHierarchyQuery request,
        CancellationToken ct)
    {
        // 1️⃣ Load case
        var c = await _db.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == request.CaseId, ct);

        if (c == null)
            return ApiResult<List<CaseWorkflowStepDto>>
                .Fail("Case not found.");

        // 2️⃣ Load FULL workflow steps (definition)
        var steps = await _db.Set<ProcessStep>()
            .Include(s => s.Directorate)
            .Where(s => s.ProcessId == c.ProcessId)
            .OrderBy(s => s.StepNo)
            .ToListAsync(ct);

        // 3️⃣ Load step history (actual actions)
        var history = await _db.Set<CaseStepHistory>()
            .Where(h => h.CaseId == c.Id)
            .ToListAsync(ct);

        // 4️⃣ Build hierarchy
        var result = steps.Select(step =>
        {
            var historyForStep = history
                .Where(h => h.StepId == step.Id)
                .OrderByDescending(h => h.Created)
                .FirstOrDefault();

            WorkflowStepStatus status;

            if (c.Status == CaseStatus.Rejected && step.StepNo == c.CurrentStepNo)
            {
                status = WorkflowStepStatus.Rejected;
            }
            else if (c.Status == CaseStatus.Approved)
            {
                status = WorkflowStepStatus.Approved;
            }
            else if (step.StepNo < c.CurrentStepNo)
            {
                status = WorkflowStepStatus.Approved;
            }
            else if (step.StepNo == c.CurrentStepNo)
            {
                status = WorkflowStepStatus.InProgress;
            }
            else
            {
                status = WorkflowStepStatus.NotStarted;
            }

            return new CaseWorkflowStepDto(
                step.StepNo,
                step.Name,
                step.Directorate.Name,
                status,
                historyForStep?.Created,
                historyForStep?.Remarks
            );
        })
        .ToList();

        return ApiResult<List<CaseWorkflowStepDto>>.Ok(result);
    }
}


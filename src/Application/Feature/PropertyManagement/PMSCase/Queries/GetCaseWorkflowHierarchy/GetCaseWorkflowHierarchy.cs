using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

public class FinalWorkFlowWithFee
{
    public List<CaseWorkflowStepDto> Steps { get; set; } = new();
    public PaymentStatusDto PaymentStatusDto { get; set; } = new();

    public List<CaseResultDocumentDto> ResultDocuments { get; set; } = new();
}
public class CaseResultDocumentDto
{
    public string DocumentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string DownloadUrl { get; set; } = default!;
}


public record CaseWorkflowStepDto
(
    int StepNo,
    string StepName,
    string DirectorateName,
    WorkflowStepStatus Status,
    DateTime? ActionDate,
    string? Remarks
);
public class PaymentStatusDto
{
    public bool IsPayment { get; set; }     // Fee receipt exists
    public bool IsRequired { get; set; }    // Fee required for process
    public string? Status { get; set; }
    public PaymentInfoDto? Payment { get; set; }
}
public class PaymentInfoDto
{
    public decimal? Amount { get; set; }
    public string Currency { get; set; } = "PKR";
    public string Purpose { get; set; } = default!;
}

public record GetCaseWorkflowHierarchyQuery(Guid CaseId)
    : IRequest<ApiResult<FinalWorkFlowWithFee>>;

public class GetCaseWorkflowHierarchyHandler
    : IRequestHandler<GetCaseWorkflowHierarchyQuery, ApiResult<FinalWorkFlowWithFee>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorageService;
    private readonly IApplicationDbContext _applicationDbContext;

    public GetCaseWorkflowHierarchyHandler(IPMSApplicationDbContext db, IFileStorageService fileStorageService, IApplicationDbContext applicationDbContext)
    {
        _db = db;
        _fileStorageService = fileStorageService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<ApiResult<FinalWorkFlowWithFee>> Handle(GetCaseWorkflowHierarchyQuery request, CancellationToken ct)
    {
        // 1️⃣ Load case
        var c = await _db.Set<PropertyCase>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(
                x => x.Id == request.CaseId && x.IsDeleted != true,
                ct);

        if (c == null)
            return ApiResult<FinalWorkFlowWithFee>.Fail("Case not found.");

        // 2️⃣ Load workflow steps (definition)
        var steps = await _db.Set<ProcessStep>()
            .Include(s => s.Directorate)
            .Where(s => s.ProcessId == c.ProcessId && s.IsDeleted != true)
            .OrderBy(s => s.StepNo)
            .ToListAsync(ct);

        // 3️⃣ Load step history
        var history = await _db.Set<CaseStepHistory>()
            .Where(h => h.CaseId == c.Id && h.IsDeleted != true)
            .ToListAsync(ct);

        // 4️⃣ Load fee receipt (latest)
        var fee = await _db.Set<CaseFeeReceipt>()
         .Where(x => x.CaseId == c.Id && x.IsDeleted != true)
         .OrderByDescending(x => x.Created)
         .FirstOrDefaultAsync(ct);

        // 🔁 Re-verify if still pending & no transaction id
        if (fee != null &&
            fee.PaymentStatus == PaymentStatus.Pending &&
            string.IsNullOrEmpty(fee.GatewayTransactionId))
        {
            var ipn = await _applicationDbContext.Set<PaymentIpnLog>()
                .Where(x =>
                    x.BasketId == fee.BankRefNo &&
                    x.ErrMsg == "Success")
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(ct);

            if (ipn != null)
            {
                fee.GatewayTransactionId = ipn.Id.ToString();
                fee.PaidAmount = ipn.Amount;
                fee.PaymentStatus = PaymentStatus.Paid;

                await _db.SaveChangesAsync(ct);
            }
        }

        // default flags
        bool isPayment = false;
        bool isRequired = false;
        string statusText = "NotRequired";

        if (fee != null)
        {
            isPayment = true;

            if (fee.PaymentStatus == PaymentStatus.Paid)
            {
                isRequired = false;
                statusText = "Paid";
            }
            else // Pending / Failed / any non-paid
            {
                isRequired = true;
                statusText = fee.PaymentStatus.ToString(); // "Pending", "Failed"
            }
        }

        var finalPaymentStatus = new PaymentStatusDto
        {
            IsPayment = isPayment,
            IsRequired = isRequired,
            Status = statusText,

            Payment = fee != null
                ? new PaymentInfoDto
                {
                    Amount = fee.PaidAmount ?? fee.TotalPayable ?? 0,
                    Currency = "PKR",
                    Purpose = c.Process.Name
                }
                : null
        };



        // 6️⃣ Build workflow hierarchy
        var workflowSteps = steps.Select(step =>
        {
            var historyForStep = history
                .Where(h => h.StepId == step.Id)
                .OrderByDescending(h => h.Created)
                .FirstOrDefault();

            WorkflowStepStatus status;

            if (c.Status == CaseStatus.Rejected && step.StepNo == c.CurrentStepNo)
                status = WorkflowStepStatus.Rejected;
            else if (c.Status == CaseStatus.Approved)
                status = WorkflowStepStatus.Approved;
            else if (step.StepNo < c.CurrentStepNo)
                status = WorkflowStepStatus.Approved;
            else if (step.StepNo == c.CurrentStepNo)
                status = WorkflowStepStatus.InProgress;
            else
                status = WorkflowStepStatus.NotStarted;

            // ✅ Attach payment ONLY on fee step (optional rule)
            

            return new CaseWorkflowStepDto(
                step.StepNo,
                step.Name,
                step.Directorate.Name,
                status,
                historyForStep?.Created,
                historyForStep?.Remarks
            );
        }).ToList();

        var documents = await _db.Set<CaseResultDocument>()
        .AsNoTracking()
        .Where(x => x.CaseId == c.Id && x.IsFinal)
        .ToListAsync(ct);

        var documentDtos = documents.Select(d => new CaseResultDocumentDto
        {
            DocumentType = d.DocumentType,
            FileName = d.FileName,
            DownloadUrl = _fileStorageService.GetPublicUrl(d.FilePath)
        }).ToList();

        // 7️⃣ Final response
        return ApiResult<FinalWorkFlowWithFee>.Ok(new FinalWorkFlowWithFee
        {
            Steps = workflowSteps,
            PaymentStatusDto = finalPaymentStatus,
            ResultDocuments = documentDtos
        });
    }
}


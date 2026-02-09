using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class FinalWorkFlowWithFee
{
    public List<CaseWorkflowStepDto> Steps { get; set; } = new();
    public PaymentStatusDto PaymentStatusDto { get; set; } = new();
    public List<CaseResultDocumentDto> ResultDocuments { get; set; } = new();
    // ONLY when rejected
    public CaseRejectionDto? RejectedPrerequisites { get; set; }
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
    string? Remarks,
    string? ActionBy
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

public class CaseRejectionDto
{
    public string Remarks { get; set; } = default!;
    public List<RejectionProcessPrerequisiteDto> Requirements { get; set; } = new();
}

public record GetCaseWorkflowHierarchyQuery(Guid CaseId)
    : IRequest<ApiResult<FinalWorkFlowWithFee>>;

public class GetCaseWorkflowHierarchyHandler
    : IRequestHandler<GetCaseWorkflowHierarchyQuery, ApiResult<FinalWorkFlowWithFee>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorageService;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentDbContext _paymentDbContext;

    public GetCaseWorkflowHierarchyHandler(IPMSApplicationDbContext db, IFileStorageService fileStorageService, IApplicationDbContext applicationDbContext,UserManager<ApplicationUser> userManager, IPaymentDbContext paymentDbContext)
    {
        _db = db;
        _fileStorageService = fileStorageService;
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _paymentDbContext = paymentDbContext;
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
            .Where(s => s.ProcessId == c.ProcessId && c.IsActive==true && s.IsDeleted != true)
            .OrderBy(s => s.StepNo)
            .ToListAsync(ct);

        // 3️⃣ Load step history
        var history = await _db.Set<CaseStepHistory>()
            .Where(h => h.CaseId == c.Id && c.IsActive == true && h.IsDeleted != true)
            .ToListAsync(ct);

        // 4️⃣ Load fee receipt (latest)
        var fee = await _db.Set<CaseFeeReceipt>()
         .Where(x => x.CaseId == c.Id && c.IsActive == true && x.IsDeleted != true)
         .OrderByDescending(x => x.Created)
         .FirstOrDefaultAsync(ct);

        // 🔁 Re-verify if still pending & no transaction id
        if (fee != null &&
            fee.PaymentStatus == PaymentStatus.Pending &&
            string.IsNullOrEmpty(fee.GatewayTransactionId))
        {
            var ipn = await _paymentDbContext.Set<PaymentIpnLog>()
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


        CaseRejectionDto? rejectionDto = null;

        if (c.Status == CaseStatus.Rejected)
        {
            // 1️⃣ Latest rejection remarks
            var rejectionHistory = await _db.Set<CaseStepHistory>()
                .Where(x =>
                    x.CaseId == c.Id &&
                    x.Action == CaseAction.Rejected)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(ct);

            // 2️⃣ Rejected requirements (SOURCE OF TRUTH)
            var rejected = await _db.Set<CaseRejectRequirement>()
                .Include(x => x.PrerequisiteDefinition)
                    .ThenInclude(d => d.Options)
                .Where(x => x.CaseId == c.Id)
                .ToListAsync(ct);

            // 3️⃣ Map directly → SAME DTO
            var requirements = rejected.Select(r =>
            {
                var def = r.PrerequisiteDefinition;

                return new RejectionProcessPrerequisiteDto(
                    Id: new Guid(),        // ✅ not process based
                    ProcessId: c.ProcessId,             // ✅ FROM CASE
                    PrerequisiteDefinitionId: def.Id,

                    Code: def.Code,
                    Name: def.Name,
                    Type: def.Type,

                    IsRequired: true,                   // ✅ rejected docs are mandatory
                    RequiredByStepNo: 0,                // ✅ no workflow step

                    MinLength: def.MinLength,
                    MaxLength: def.MaxLength,
                    AllowedExtensions: def.AllowedExtensions,

                    Options: def.Options
                        .OrderBy(o => o.SortOrder)
                        .Select(o => new PrerequisiteOptionDto(
                            o.Id,
                            o.Label,
                            o.Value,
                            o.SortOrder
                        ))
                        .ToList(),

                    // case-specific state
                    IsUploaded: r.IsUploaded,
                    Remarks: r.Remarks,
                    UploadedDocumentId: r.UploadedDocumentId
                );
            }).ToList();

            rejectionDto = new CaseRejectionDto
            {
                Remarks = rejectionHistory?.Remarks
                    ?? "Additional information required.",
                Requirements = requirements
            };
        }
        var userIds = history
    .GroupBy(h => h.StepId)
    .Select(g => g
        .OrderByDescending(x => x.Created)
        .FirstOrDefault()?.CreatedBy)
    .Where(id => id != null)
    .Distinct()
    .ToList();




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
                historyForStep?.Remarks,
                ResolveUserName(historyForStep?.CreatedBy)
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
            ResultDocuments = documentDtos,

            RejectedPrerequisites = rejectionDto   // 🔴 IMPORTANT
        });
    }

    string? ResolveUserName(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        return _userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Name ?? u.Name)
            .FirstOrDefault();
    }

}


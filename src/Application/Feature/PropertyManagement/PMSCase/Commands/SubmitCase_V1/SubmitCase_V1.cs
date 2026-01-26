using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.SubmitCase_V1;
public record SubmitCase_V1Command(
    Guid UserPropertyId,
    Guid ProcessId,
    string? ApplicantRemarks,
    List<PrerequisiteValueInput> PrerequisiteValues,
    List<IFormFile>? Files,
    // Fee
    Guid? FeeDefinitionId,
    Guid? FeeOptionId,
    string? BankRefNo
) : IRequest<ApiResult<SubmitCaseResponse>>;

public class SubmitCase_V1Handler: IRequestHandler<SubmitCase_V1Command, ApiResult<SubmitCaseResponse>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SubmitCase_V1Handler(
        IPMSApplicationDbContext db,
        IFileStorageService fileStorage,
        IApplicationDbContext applicationDbContext,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _fileStorage = fileStorage;
        _applicationDbContext = applicationDbContext;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<ApiResult<SubmitCaseResponse>> Handle(
        SubmitCase_V1Command r,
        CancellationToken ct)
    {

        var userId = _currentUserService.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new UnAuthorizedException("Invalid CNIC. Please verify and try again.");
        }
        using var trx = await _db.Database.BeginTransactionAsync(ct);

        var getProcess = await _db.Set<ServiceProcess>()
            .Where(x => x.Id == r.ProcessId && x.IsDeleted !=true)
            .FirstOrDefaultAsync(ct);

        var firstStep = await _db.Set<ProcessStep>()
            .Where(x => x.ProcessId == r.ProcessId && x.IsDeleted==false)
            .OrderBy(x => x.StepNo)
            .Select(x => new { x.DirectorateId })
            .FirstOrDefaultAsync(ct);

        if (firstStep == null)
            throw new Exception("No process steps found.");

        Guid CurrentDirectorateId = firstStep.DirectorateId;

        var getDirect = await _db.Set<Directorate>()
            .Where(x => x.Id == CurrentDirectorateId && x.IsDeleted == false)
            .Select(x => new { x.ModuleId })
            .FirstOrDefaultAsync(ct);

        if (getDirect == null)
            throw new Exception("No Module Found.");

        Guid getModuleId = getDirect.ModuleId;

        var caseNo = $"PMS-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        var c = new PropertyCase
        {
            UserPropertyId = r.UserPropertyId,
            ProcessId = r.ProcessId,
            CaseNo = caseNo,
            Status = CaseStatus.Draft,
            ApplicantName = user.Name ?? "",
            ApplicantCnic = user.CNIC,
            ApplicantMobile = user.RegisteredMobileNo,
            ApplicantRemarks = r.ApplicantRemarks,
            CurrentModuleId= getModuleId,
            DirectorateId = CurrentDirectorateId,
        };

            _db.Set<PropertyCase>().Add(c);
            await _db.SaveChangesAsync(ct);


        // =========================
        // 2️⃣ SAVE PREREQUISITE VALUES
        // =========================

        // 1️⃣ Get all prerequisites attached to this process
        var getAllPre = await _db.Set<ProcessPrerequisite>()
            .Include(x => x.PrerequisiteDefinition)
            .Where(x => x.ProcessId == r.ProcessId && x.IsActive==true && x.IsDeleted !=true)
            .ToListAsync(ct);

        // 2️⃣ Build allowed prerequisite IDs (exclude StaticLabel)
        var allowedPrerequisiteIds = getAllPre
            .Where(x => x.PrerequisiteDefinition.Type != PrerequisiteType.StaticLabel)
            .Select(x => x.PrerequisiteDefinitionId)
            .ToHashSet();

        // 3️⃣ Filter incoming values (only valid + non-static)
        var validValues = r.PrerequisiteValues
            .Where(p => allowedPrerequisiteIds.Contains(p.PrerequisiteDefinitionId))
            .ToList();

        // 4️ Insert values
        foreach (var p in validValues)
        {
            _db.Set<CasePrerequisiteValue>().Add(new CasePrerequisiteValue
            {
                CaseId = c.Id,
                PrerequisiteDefinitionId = p.PrerequisiteDefinitionId,
                ValueText = p.ValueText,
                ValueNumber = p.ValueNumber,
                ValueDate = p.ValueDate,
                ValueBool = p.ValueBool
            });
        }
        await _db.SaveChangesAsync(ct);

        // =========================
        // 3️ SAVE FILES
        // Filename format: {PrerequisiteDefinitionId}__filename.ext
        // =========================
        if (r.Files != null)
        {
            foreach (var file in r.Files)
            {
                Guid? prereqId = null;
                var parts = file.FileName.Split("__", 2);

                if (parts.Length == 2 && Guid.TryParse(parts[0], out var pid))
                    prereqId = pid;

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

                var path = await _fileStorage.SavePMSDocumentAsync(
                    file,
                    $"pms/cases/{c.Id}",
                    ct,
                    maxBytes: 10 * 1024 * 1024,
                    allowedExtensions: allowedExtensions
                );

               
                _db.Set<CaseDocument>().Add(new CaseDocument
                {
                    CaseId = c.Id,
                    PrerequisiteDefinitionId = prereqId,
                    FileName = file.FileName,
                    FileUrl = path,
                    ContentType = file.ContentType,
                    FileSize = file.Length
                });
            }
        }

        await _db.SaveChangesAsync(ct);

        // =========================
        // 4️ VALIDATION
        // =========================

        var processPrereqs = await _db.Set<ProcessPrerequisite>()
            .Include(x => x.PrerequisiteDefinition)
            .Where(x =>
                x.ProcessId == r.ProcessId && x.IsActive==true &&
                x.RequiredByStepNo == 0 && 
                x.PrerequisiteDefinition.Type != PrerequisiteType.StaticLabel)
            .ToListAsync(ct);

        var providedValueIds = await _db.Set<CasePrerequisiteValue>()
            .Where(x => x.CaseId == c.Id && x.IsActive==true)
            .Select(x => x.PrerequisiteDefinitionId)
            .ToListAsync(ct);

        var providedFileIds = await _db.Set<CaseDocument>()
            .Where(x => x.CaseId == c.Id && x.PrerequisiteDefinitionId != null && x.IsActive==true)
            .Select(x => x.PrerequisiteDefinitionId!.Value)
            .ToListAsync(ct);

        var providedAll = providedValueIds
            .Union(providedFileIds)
            .Distinct()
            .ToList();

        // ❌ Missing required
        var missing = processPrereqs
            .Where(x => x.IsRequired && !providedAll.Contains(x.PrerequisiteDefinitionId))
            .ToList();

        if (missing.Any())
            return ApiResult<SubmitCaseResponse>.Fail(
                $"Missing required prerequisites: {missing.Count}");

        // =========================
        // 4️ OPTION VALIDATION
        // =========================
        foreach (var value in r.PrerequisiteValues)
        {
            var def = await _db.Set<PrerequisiteDefinition>()
                .FirstAsync(x => x.Id == value.PrerequisiteDefinitionId, ct);

            if (!IsOptionBased(def.Type))
                continue;

            var allowed = await _db.Set<PrerequisiteOption>()
                .Where(x => x.PrerequisiteDefinitionId == def.Id && !x.IsDisabled)
                .Select(x => x.Value)
                .ToListAsync(ct);

            if (def.Type is PrerequisiteType.Dropdown or PrerequisiteType.RadioGroup)
            {
                if (string.IsNullOrWhiteSpace(value.ValueText) ||
                    !allowed.Contains(value.ValueText))
                    return ApiResult<SubmitCaseResponse>.Fail(
                        $"Invalid value for {def.Name}");
            }

            if (def.Type is PrerequisiteType.MultiSelect or PrerequisiteType.CheckboxGroup)
            {
                var selected = JsonSerializer.Deserialize<List<string>>(value.ValueText ?? "[]");
                if (selected == null || selected.Any(x => !allowed.Contains(x)))
                    return ApiResult<SubmitCaseResponse>.Fail(
                        $"Invalid selection for {def.Name}");
            }
        }
        // 5 Resolve Fee
        if (r.FeeDefinitionId is { } id && id != Guid.Empty)
        {
            var feeDef = await _db.Set<FeeDefinition>()
                .FirstOrDefaultAsync(x =>
                    x.Id == r.FeeDefinitionId &&
                    x.IsDeleted != true &&
                    x.IsActive == true,
                    ct);

            if (feeDef == null)
                return ApiResult<SubmitCaseResponse>.Fail("Fee definition not found.");

            decimal? finalAmount;
            decimal nadrafee = 0;
            decimal servicefee = 0;
            int processingDays = 0;
            Guid? selectedOptionId = null;

            switch (feeDef.FeeType)
            {
                case FeeType.Fixed:
                    finalAmount = feeDef.FixedAmount!.Value;
                    break;

                case FeeType.OptionBased:
                case FeeType.OptionBasedWithCategory:
                    {
                        if (r.FeeOptionId == null)
                            return ApiResult<SubmitCaseResponse>.Fail("Fee option required.");

                        var option = await _db.Set<FeeOption>()
                            .FirstOrDefaultAsync(x =>
                                x.Id == r.FeeOptionId &&
                                x.FeeDefinitionId == feeDef.Id &&
                                x.IsDeleted != true,
                                ct);

                        if (option == null)
                            return ApiResult<SubmitCaseResponse>.Fail("Invalid fee option.");

                        finalAmount = option.Amount;
                        processingDays = option.ProcessingDays;
                        selectedOptionId = option.Id;
                        break;
                    }


                case FeeType.Manual:
                    {

                        finalAmount = feeDef.FixedAmount;
                        break;
                    }

                default:
                    return ApiResult<SubmitCaseResponse>.Fail("Unsupported fee type.");
            }

            decimal? totalPayable = finalAmount;

            // NADRA Fee
            if (getProcess != null)
            {
                if (getProcess.IsNadraVerificationRequired)
                {
                    var nadraFee = await _db.Set<FeeSetting>()
                        .FirstOrDefaultAsync(x =>
                            x.Code == "NADRA_FEE" &&
                            x.IsDeleted != true &&
                            x.IsActive == true,
                            ct);

                    if (nadraFee != null)
                    {
                        nadrafee = nadraFee.Amount;
                        totalPayable += nadraFee.Amount;
                    }
                }
            }

            // Service Fee (always)
            var serviceFee = await _db.Set<FeeSetting>()
                .FirstOrDefaultAsync(x =>
                    x.Code == "SERVICE_FEE" &&
                    x.IsDeleted != true &&
                    x.IsActive == true,
                    ct);

            if (serviceFee != null)
            {
                totalPayable += serviceFee.Amount;
                servicefee = serviceFee.Amount;
            }
            // 5️6 Save CaseFee snapshot

            var getTransactionlogs = await _applicationDbContext.Set<PaymentIpnLog>()
                .Where(x => x.BasketId == r.BankRefNo && x.ErrMsg == "Success")
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(ct);

            _db.Set<CaseFeeReceipt>().Add(new CaseFeeReceipt
            {
                CaseId = c.Id,
                FeeDefinitionId = feeDef.Id,
                FeeOptionId = selectedOptionId,
                PaidAmount = getTransactionlogs?.Amount,
                GatewayTransactionId = getTransactionlogs?.Id.ToString(),
                NADRA_FEE = nadrafee,
                SERVICE_FEE = servicefee,
                PaymentMethod = PaymentMethod.Online,
                TotalPayable = totalPayable,
                BankRefNo = r.BankRefNo,
                PaymentStatus= getTransactionlogs != null ? PaymentStatus.Paid : PaymentStatus.Pending
            });
            await _db.SaveChangesAsync(ct);
        }

        // =========================
        // 6 MOVE TO STEP-1
        // =========================
        var step1 = await _db.Set<ProcessStep>()
        .Include(x => x.Directorate)
        .FirstAsync(x => x.ProcessId == r.ProcessId && x.StepNo == 1, ct);

        c.Status = CaseStatus.Submitted;
        c.CurrentStepNo = 1;
        c.CurrentStepId = step1.Id;

        _db.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,

            StepId = step1.Id,
            StepNo = step1.StepNo,
            StepName = step1.Name,

            DirectorateId = step1.DirectorateId,
            DirectorateName = step1.Directorate.Name,

            ModuleId = step1.Directorate.ModuleId,

            Action = CaseAction.Received,
            Remarks = "Application received.",
            PerformedByUserId = userId
        });

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<SubmitCaseResponse>.Ok(
            new SubmitCaseResponse(c.Id, caseNo),
            "Case submitted successfully");
    }

    private static bool IsOptionBased(PrerequisiteType type) =>
        type == PrerequisiteType.Dropdown ||
        type == PrerequisiteType.MultiSelect ||
        type == PrerequisiteType.CheckboxGroup ||
        type == PrerequisiteType.RadioGroup;
}

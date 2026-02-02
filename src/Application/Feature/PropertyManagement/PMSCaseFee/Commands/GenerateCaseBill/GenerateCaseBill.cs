using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.GenerateCaseBill;
public record GenerateCaseBillCommand(
    Guid CaseId,
    Guid FeeDefinitionId,
    Guid? FeeOptionId    // required only for option-based fees
) : IRequest<ApiResult<Guid>>;
public class GenerateCaseBillHandler
    : IRequestHandler<GenerateCaseBillCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly ISmsService _sms;
    private readonly INotificationService _notify;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _appDb;
    private readonly IPaymentBillService _paymentBillService;

    public GenerateCaseBillHandler(IPMSApplicationDbContext db, ISmsService sms, INotificationService notify, UserManager<ApplicationUser> userManager, IApplicationDbContext appDb, IPaymentBillService paymentBillService)
    {
        _db = db;
        _sms = sms;
        _notify = notify;
        _userManager = userManager;
        _appDb = appDb;
        _paymentBillService = paymentBillService;
    }

    public async Task<ApiResult<Guid>> Handle(
        GenerateCaseBillCommand r,
        CancellationToken ct)
    {
        // =========================
        // 1️⃣ Load Case
        // =========================
        var c = await _db.Set<PropertyCase>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(x => x.Id == r.CaseId && x.IsDeleted !=true, ct);

        if (c == null)
            return ApiResult<Guid>.Fail("Case not found.");

        // ❌ Prevent duplicate pending bill
        var alreadyPending = await _db.Set<CaseFeeReceipt>()
            .AnyAsync(x =>
                x.CaseId == c.Id &&
                x.PaymentStatus == PaymentStatus.Pending &&
                x.IsDeleted !=true,
                ct);

        if (alreadyPending)
            return ApiResult<Guid>.Fail("A pending bill already exists for this case.");

        // =========================
        // 2️⃣ Load Fee Definition
        // =========================
        var feeDef = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.FeeDefinitionId &&
                x.IsActive == true &&
                x.IsDeleted != true,
                ct);

        if (feeDef == null)
            return ApiResult<Guid>.Fail("Fee definition not found.");

        decimal baseAmount;
        Guid? selectedOptionId = null;

        // =========================
        // 3️⃣ Resolve BASE AMOUNT (USER SELECTED)
        // =========================
        switch (feeDef.FeeType)
        {
            case FeeType.Fixed:
                baseAmount = feeDef.FixedAmount ?? 0;
                break;

            case FeeType.OptionBased:
            case FeeType.OptionBasedWithCategory:
                {
                    if (r.FeeOptionId == null)
                        return ApiResult<Guid>.Fail("Fee option is required.");

                    var option = await _db.Set<FeeOption>()
                        .FirstOrDefaultAsync(x =>
                            x.Id == r.FeeOptionId &&
                            x.FeeDefinitionId == feeDef.Id &&
                            x.IsDeleted !=true,
                            ct);

                    if (option == null)
                        return ApiResult<Guid>.Fail("Invalid fee option.");

                    baseAmount = option.Amount;
                    selectedOptionId = option.Id;
                    break;
                }

            case FeeType.Manual:
                baseAmount = feeDef.FixedAmount ?? 0;
                break;

            default:
                return ApiResult<Guid>.Fail("Unsupported fee type.");
        }

        // =========================
        // 4️⃣ CALCULATE NADRA + SERVICE FEES (PROCESS BASED)
        // =========================
        decimal totalPayable = baseAmount;
        decimal nadraFeeAmount = 0;
        decimal serviceFeeAmount = 0;

        // 🔹 NADRA fee (only if process requires)
        if (c.Process.IsNadraVerificationRequired)
        {
            var nadraFee = await _db.Set<FeeSetting>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Code == "NADRA_FEE" &&
                    x.IsActive ==true &&
                    x.IsDeleted !=true,
                    ct);

            if (nadraFee != null)
            {
                nadraFeeAmount = nadraFee.Amount;
                totalPayable += nadraFeeAmount;
            }
        }

        // 🔹 Service fee (always)
        var serviceFee = await _db.Set<FeeSetting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Code == "SERVICE_FEE" &&
                x.IsActive ==true &&
                x.IsDeleted !=true,
                ct);

        if (serviceFee != null)
        {
            serviceFeeAmount = serviceFee.Amount;
            totalPayable += serviceFeeAmount;
        }

        // =========================
        // 5️⃣ CREATE BILL (PENDING)
        // =========================
        var bill = new CaseFeeReceipt
        {
            CaseId = c.Id,
            FeeDefinitionId = feeDef.Id,
            FeeOptionId = selectedOptionId,

            TotalPayable = totalPayable,

            NADRA_FEE = nadraFeeAmount,
            SERVICE_FEE = serviceFeeAmount,

            PaymentMethod = PaymentMethod.Online,
            PaymentStatus = PaymentStatus.Pending
        };

        _db.Set<CaseFeeReceipt>().Add(bill);
        await _db.SaveChangesAsync(ct);

        await _paymentBillService.CreatePaymentBillAsync(
           new CreatePaymentBillRequest
           {
               SourceSystem = "PMS",
               SourceVoucherId = bill.Id,
               SourceVoucherNo = bill.VoucherNo ?? bill.Id.ToString(),

               Title = $"{c.Process.Name} - {c.CaseNo}",

               EntityType = "Property",
               EntityId = c.UserPropertyId,
               EntityName = c.Process.Name.ToString(),

               UserId = c.CreatedBy!,
               TotalAmount = totalPayable,

               DueDate = bill.DueDate
           },
           ct);

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == c.CreatedBy);
        if (user != null)
        {
            UserDevices? getToken = null;

            if (!string.IsNullOrWhiteSpace(c.CreatedBy) &&
                Guid.TryParse(c.CreatedBy, out var createdByUserId))
            {
                getToken = await _appDb.Set<UserDevices>()
                .Where(x => x.UserId == createdByUserId && x.IsActive ==true && x.IsDeleted !=true)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(ct);
            }

            if (getToken == null)
                return ApiResult<Guid>.Fail("User not found.");
            if (getToken.FCMToken == null)
                return ApiResult<Guid>.Fail("User Token found.");

            var message = $"Dear {user.Name}, a new bill of amount {totalPayable:C} has been generated for your case (ID: {c.Id}). Please proceed to payment in mobile application.";
            await _notify.SendFirebasePMSNotificationAsync(getToken.FCMToken, "New Bill Generated", message, ct);

            if(user.RegisteredMobileNo !=null)
                await _sms.SendSmsAsync(user.RegisteredMobileNo, message, ct);
        }

        return ApiResult<Guid>.Ok(bill.Id, "Bill generated successfully.");
    }
}

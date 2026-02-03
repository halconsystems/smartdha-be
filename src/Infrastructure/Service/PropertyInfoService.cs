using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.Payment;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class PropertyInfoService : IPropertyInfoService
{
    private readonly IProcedureService _procedureService;
    private readonly IPaymentDbContext _db;
    private readonly ILateFeePolicyResolver _lateFeePolicyResolver;
    public PropertyInfoService(IProcedureService procedureService, IPaymentDbContext db, ILateFeePolicyResolver lateFeePolicyResolver)
    {
        _procedureService = procedureService;
        _db = db;
        _lateFeePolicyResolver = lateFeePolicyResolver;
    }

    public async Task<List<SmartPayBillData>> GetPendingBillsByUserAsync(
    string userId,
    CancellationToken ct)
    {
        var now = DateTime.Now;

        var bills = await _db.PayBills
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                (x.PaymentStatus == PaymentBillStatus.Generated ||
                 x.PaymentStatus == PaymentBillStatus.PartiallyPaid) 
                 && (x.ExpiryDate == null || x.ExpiryDate > now)
                 )
            .OrderByDescending(x => x.BillGeneratedOn)
            .ToListAsync(ct);

        var result = new List<SmartPayBillData>();

        foreach (var bill in bills)
        {
            // 🔹 Resolve policy PER BILL (PMS / CLUB / SCHOOL)
            var policy = await _lateFeePolicyResolver
                .ResolveAsync(bill.SourceSystem, ct);

            // 🔹 Calculate late fee (read-only)
            var lateFee = CalculateLateFee(bill, policy, now);
            var amountAfterDueDate = bill.BillAmount + lateFee;

            result.Add(new SmartPayBillData
            {
                // 🔹 Consumer Info
                Institution = bill.Title,
                Consumer_Number = "",
                Consumer_Detail = bill.EntityName ?? string.Empty,

                // 🔹 Bill Info
                Reference_Info = !string.IsNullOrWhiteSpace(bill.SourceVoucherNo)
                    ? bill.SourceVoucherNo
                    : bill.PaymentBillId.ToString(),

                Billing_Month = bill.BillGeneratedOn.ToString("yyyy-MM"),

                // 🔹 Amounts (calculated)
                BillAmount = bill.BillAmount.ToString("0.##"),
                LateFee = lateFee.ToString("0.##"),
                AmountAfterDueDate = amountAfterDueDate.ToString("0.##"),

                // 🔹 Dates
                DueDate = bill.DueDate.HasValue
                    ? bill.DueDate.Value.ToString("yyyy-MM-dd")
                    : string.Empty,

                ExpDate = bill.ExpiryDate.HasValue
                    ? bill.ExpiryDate.Value.ToString("yyyy-MM-dd")
                    : string.Empty,

                // 🔹 Status
                PaymentStatus = bill.PaymentStatus.ToString(),
                Amount_Paid = bill.PaidAmount.ToString("0.##"),

                PaymentDateTime = bill.LastPaymentDate?.ToString("yyyy-MM-dd"),

                // 🔹 Gateway
                AuthNo = bill.LastAuthNo ?? string.Empty,

                Fee_Amount = bill.BillAmount.ToString("0.##"),
                BillGenerateOn = bill.BillGeneratedOn.ToString("yyyy-MM-dd")
            });
        }

        return result;
    }



    public async Task<List<PropertyDetailDTO>> GetPropertiesByCnicAsync(
        string cnic,
        CancellationToken ct)
    {
        var parameters = new DynamicParameters();

        cnic = "3220384127171";
        //parameters.Add("@memCNIC", cnic, DbType.String, size: 150);
        parameters.Add("@memCNIC", cnic, DbType.String, size: 150);
        parameters.Add("@msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        var (_, data) = await _procedureService.ExecuteWithListAsync<PropertyDetailDTO>(
            "USP_SelectPropertyByCNIC",
            parameters,
            ct
        );

        return data.Select(x => new PropertyDetailDTO
        {
            PLOT_NO = x.PLOT_NO,
            STNAME = x.STNAME,
            PLTNO = x.PLTNO,
            SUBDIV = x.SUBDIV,
            PTYPE = x.PTYPE,
            PHASE = x.PHASE,
            EXT = x.EXT,
            NOMEAEA = x.NOMEAEA,
            ACTUAL_SIZE = x.ACTUAL_SIZE,
            STREET1COD = x.STREET1COD,
            PLOTPK = x.PLOTPK,
            MEMPK = x.MEMPK,
            MEMNO = x.MEMNO,
            CAT = x.CAT,
            NAME = x.NAME,
            APPLIDATE = x.APPLIDATE,
            NIC = x.NIC,
            CELLNO = x.CELLNO,
            ALLRESPLOT = x.ALLRESPLOT,
        }).ToList();
    }


    private static decimal CalculateLateFee(
    PayBill bill,
    PayLateFeePolicy policy,
    DateTime nowUtc)
    {
        if (!bill.DueDate.HasValue)
            return 0;

        var lateStartDate = bill.DueDate.Value.AddDays(policy.GraceDays);

        if (nowUtc <= lateStartDate)
            return 0;

        return policy.LateFeeType switch
        {
            LateFeeType.Fixed => policy.FixedLateFee,

            LateFeeType.PerDay =>
                (decimal)(nowUtc.Date - lateStartDate.Date).TotalDays
                * policy.PerDayLateFee,

            _ => 0
        };
    }

}

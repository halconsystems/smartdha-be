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
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.Payment;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class PropertyInfoService : IPropertyInfoService
{
    private readonly IProcedureService _procedureService;
    private readonly IPaymentDbContext _db;
    public PropertyInfoService(IProcedureService procedureService, IPaymentDbContext db)
    {
        _procedureService = procedureService;
        _db = db;
    }

    public async Task<List<SmartPayBillData>> GetPendingBillsByUserAsync(
     string userId,
     CancellationToken ct)
    {
        var bills = await _db.PayBills
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                (x.PaymentStatus == PaymentBillStatus.Generated ||
                 x.PaymentStatus == PaymentBillStatus.PartiallyPaid) &&
                (x.ExpiryDate == null || x.ExpiryDate > DateTime.Now))
            .OrderByDescending(x => x.BillGeneratedOn)
            .ToListAsync(ct);

        return bills.Select(bill =>
        {
            var reference = !string.IsNullOrWhiteSpace(bill.SourceVoucherNo)
                ? bill.SourceVoucherNo
                : bill.PaymentBillId.ToString();

            return new SmartPayBillData
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

                // 🔹 Amounts (DECIMAL → STRING)
                BillAmount = bill.BillAmount.ToString("0.##"),
                LateFee = "0", // Late fee is calculated in source system
                AmountAfterDueDate = bill.BillAmount.ToString("0.##"),

                // 🔹 Dates (DateTime → STRING)
                DueDate = bill.DueDate.HasValue
                ? bill.DueDate.Value.ToString("yyyy-MM-dd")
                : string.Empty,
                
                        ExpDate = bill.ExpiryDate.HasValue
                ? bill.ExpiryDate.Value.ToString("yyyy-MM-dd")
                : string.Empty,
                
                        // 🔹 Payment Status
                        PaymentStatus = bill.PaymentStatus.ToString(),
                
                        Amount_Paid = bill.PaidAmount.ToString("0.##"),
                
                        PaymentDateTime = bill.LastPaymentDate?.ToString("yyyy-MM-dd"),
                
                        // 🔹 Gateway Info
                        AuthNo = bill.LastAuthNo ?? string.Empty,
                
                        Fee_Amount = bill.BillAmount.ToString("0.##"),
                
                        BillGenerateOn = bill.Created.ToString("yyyy-MM-dd")
                    };
                
        }).ToList();
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
    private static string BuildConsumerDetail(
            string? subDivision,
            string? phase,
            string? streetName,
            string? plotNo,
            string? actualSize)
    {
        return string.Join(", ",
            new[]
            {
                subDivision,
                phase,
                streetName,
                plotNo,
                actualSize
            }.Where(x => !string.IsNullOrWhiteSpace(x))
        );
    }

}

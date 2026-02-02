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
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class PropertyInfoService : IPropertyInfoService
{
    private readonly IProcedureService _procedureService;
    private readonly IPMSApplicationDbContext _db;
    public PropertyInfoService(IProcedureService procedureService, IPMSApplicationDbContext db)
    {
        _procedureService = procedureService;
        _db = db;
    }

    public async Task<List<SmartPayBillData>> GetPendingBillsByUserAsync(
        string userId,
        CancellationToken ct)
    {
        var rows = await (
            from bill in _db.Set<CaseFeeReceipt>().AsNoTracking()
            join c in _db.Set<PropertyCase>().AsNoTracking()
                on bill.CaseId equals c.Id
            join p in _db.Set<UserProperty>().AsNoTracking()
                on c.UserPropertyId equals p.Id
            join proc in _db.Set<ServiceProcess>().AsNoTracking()
                on c.ProcessId equals proc.Id
            where
                bill.IsActive == true &&
                bill.IsDeleted != true &&
                bill.PaymentStatus == PaymentStatus.Pending &&

                c.IsActive == true &&
                c.IsDeleted != true &&
                c.CreatedBy == userId

            orderby bill.Created descending
            select new
            {
                bill.Id,
                bill.TotalPayable,
                bill.Created,
                bill.OneBillId,
                bill.VoucherNo,

                CaseNo = c.CaseNo,
                ProcessName = proc.Name,

                p.SubDivision,
                p.Phase,
                p.StreetName,
                p.PlotNo,
                p.ActualSize
            }
        ).ToListAsync(ct);

        return rows.Select(x =>
        {
            var consumerDetail = BuildConsumerDetail(
                x.SubDivision,
                x.Phase,
                x.StreetName,
                x.PlotNo,
                x.ActualSize
            );

            var reference = !string.IsNullOrWhiteSpace(x.VoucherNo)
                ? x.VoucherNo!
                : !string.IsNullOrWhiteSpace(x.OneBillId)
                    ? x.OneBillId!
                    : $"BILL-{x.Created:yyyyMMddHHmmss}";

            var amount = (x.TotalPayable ?? 0m).ToString("0.##");

            return new SmartPayBillData
            {
                Institution = x.ProcessName,
                Consumer_Number = x.CaseNo,
                Consumer_Detail = consumerDetail,
                Reference_Info = reference,
                BillAmount = amount
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

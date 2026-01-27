using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.ConsumerInquiry;
public class ConsumerInquiryQuery : IRequest<SmartPayConsumerInquiryResponse>
{
    public string CellNo { get; set; } = default!;
}
public class ConsumerInquiryQueryHandler
    : IRequestHandler<ConsumerInquiryQuery, SmartPayConsumerInquiryResponse>
{
    //private readonly ISmartPayService _smartPayService;
    private readonly IPMSApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public ConsumerInquiryQueryHandler(IPMSApplicationDbContext db, ICurrentUserService current)
    {
       // _smartPayService = smartPayService;
        _db = db;
        _current = current;
    }

    public async Task<SmartPayConsumerInquiryResponse> Handle(
        ConsumerInquiryQuery request,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            throw new ArgumentException("Unauthorized.");

        // CreatedBy in BaseAuditableEntity is string
        var userIdString = _current.UserId!.ToString();

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
               c.CreatedBy == userIdString

           orderby bill.Created descending
           select new
           {
               BillId = bill.Id,
               bill.CaseId,
               bill.TotalPayable,
               bill.Created,
               bill.OneBillId,
               bill.VoucherNo,

               CaseNo = c.CaseNo,

               ProcessName = proc.Name,

               // Property address parts
               p.PropertyNo,
               p.Phase,
               p.Sector,
               p.PlotNo,
               p.Area
           }
       ).ToListAsync(ct);

       
        var result = rows.Select(x =>
        {
            var consumerDetail = BuildConsumerDetail(x.Phase, x.Sector, x.PropertyNo, x.PlotNo, x.Area);
            // Reference priority:
            // VoucherNo -> OneBillId -> "BILL-{yyyyMMddHHmmss}"
            var reference = !string.IsNullOrWhiteSpace(x.VoucherNo)
                ? x.VoucherNo!
                : !string.IsNullOrWhiteSpace(x.OneBillId)
                    ? x.OneBillId!
                    : $"BILL-{x.Created:yyyyMMddHHmmss}";

            var amount = (x.TotalPayable ?? 0m).ToString("0.##");

            return new SmartPayConsumerInquiryBill
            {
                Institution = x.ProcessName,        // ✅ Institution = ProcessName
                Consumer_Number = x.CaseNo,         // ✅ Consumer_Number = CaseNo
                Consumer_Detail = consumerDetail,   // ✅ Property Address
                Reference_Info = reference,         // ✅ VoucherNo/OneBillId fallback
                BillAmount = amount,                // ✅ Total payable
            };
        }).ToList();

        return await Task.FromResult(
     new SmartPayConsumerInquiryResponse
     {
         ResponseCode = "00",
         ResponseMsg = "SUCCESS",
         Bills = result
     }
 );








        //if (string.IsNullOrWhiteSpace(request.CellNo))
        //    throw new ArgumentException("Cell number is required.");

        // Call SmartPay API
        //return await _smartPayService.ConsumerInquiryAsync(
        //    request.CellNo,
        //    cancellationToken);
        //return await Task.FromResult(new SmartPayConsumerInquiryResponse
        //{
        //    ResponseCode = "00",
        //    ResponseMsg = "SUCCESS",

        //    Bills = new List<SmartPayConsumerInquiryBill>
        //    {
        //         new SmartPayConsumerInquiryBill
        //          {
        //              Institution = "Security Charges",
        //              Consumer_Number = "1001",
        //              Consumer_Detail = "DHA Karachi - Phase 2",
        //              Reference_Info = "SC-2025-0001",
        //              BillAmount="10",

        //          },
        //          new SmartPayConsumerInquiryBill
        //          {
        //              Institution = "Maintenance Charges",
        //              Consumer_Number = "1002",
        //              Consumer_Detail = "DHA Karachi - Phase 7",
        //              Reference_Info = "MNT-2025-0042",
        //               BillAmount="10",
        //          },
        //          new SmartPayConsumerInquiryBill
        //          {
        //              Institution = "DA Club",
        //              Consumer_Number = "1003",
        //              Consumer_Detail = "DHA Karachi - Club Bill",
        //              Reference_Info = "DA-2025-0110",
        //               BillAmount="10",
        //          }
        //    }
        //});
    }

    static string BuildConsumerDetail(
    string? phase, string? sector, string? propertyNo, string? plotNo, string? area)
    {
        // You can adjust formatting as you like
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(sector)) parts.Add($"{sector}");
        if (!string.IsNullOrWhiteSpace(propertyNo)) parts.Add($"{propertyNo}");
        if (!string.IsNullOrWhiteSpace(plotNo)) parts.Add($"{plotNo}");
        if (!string.IsNullOrWhiteSpace(area)) parts.Add($"Area {area}");

        return parts.Count > 0 ? string.Join(" - ", parts) : "DHA Karachi";
    }
}

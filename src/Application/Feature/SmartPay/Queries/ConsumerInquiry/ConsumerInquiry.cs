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
    private readonly ISmartPayService _smartPayService;
    private readonly IPMSApplicationDbContext _db;
    private readonly ICurrentUserService _current;


    public ConsumerInquiryQueryHandler(IPMSApplicationDbContext db, ICurrentUserService current, ISmartPayService smartPayService)
    {
        _smartPayService = smartPayService;
        _db = db;
        _current = current;
    }

    public async Task<SmartPayConsumerInquiryResponse> Handle(
        ConsumerInquiryQuery request,
        CancellationToken ct)
    {

        if (string.IsNullOrWhiteSpace(request.CellNo))
            throw new ArgumentException("Cell number is required.");

        //Call SmartPay API
        return await _smartPayService.ConsumerInquiryAsync(
            request.CellNo,
            ct);
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

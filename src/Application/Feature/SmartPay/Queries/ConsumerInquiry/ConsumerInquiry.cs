using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.ConsumerInquiry;
public class ConsumerInquiryQuery : IRequest<SmartPayConsumerInquiryResponse>
{
    public string CellNo { get; set; } = default!;
}
public class ConsumerInquiryQueryHandler
    : IRequestHandler<ConsumerInquiryQuery, SmartPayConsumerInquiryResponse>
{
    //private readonly ISmartPayService _smartPayService;

    public ConsumerInquiryQueryHandler()
    {
       // _smartPayService = smartPayService;
    }

    public async Task<SmartPayConsumerInquiryResponse> Handle(
        ConsumerInquiryQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CellNo))
            throw new ArgumentException("Cell number is required.");

        // Call SmartPay API
        //return await _smartPayService.ConsumerInquiryAsync(
        //    request.CellNo,
        //    cancellationToken);
        return await Task.FromResult(new SmartPayConsumerInquiryResponse
        {
            ResponseCode = "00",
            ResponseMsg = "SUCCESS",

            Bills = new List<SmartPayConsumerInquiryBill>
            {
                 new SmartPayConsumerInquiryBill
                  {
                      Institution = "DHA Karachi",
                      Consumer_Number = "1001",
                      Consumer_Detail = "Security Charges - Phase 2",
                      Reference_Info = "SC-2025-0001",
                      BillAmount="5000",

                  },
                  new SmartPayConsumerInquiryBill
                  {
                      Institution = "DHA Karachi",
                      Consumer_Number = "1002",
                      Consumer_Detail = "Maintenance Charges - Phase 7",
                      Reference_Info = "MNT-2025-0042",
                       BillAmount="12000",
                  },
                  new SmartPayConsumerInquiryBill
                  {
                      Institution = "DHA Karachi",
                      Consumer_Number = "1003",
                      Consumer_Detail = "DA Club",
                      Reference_Info = "DA-2025-0110",
                       BillAmount="10",
                  }
            }
        });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.BillInquiry;
public class BillInquiryQuery : IRequest<SmartPayBillInquiryResponse>
{
    public string ConsumerNo { get; set; } = default!;
}
public class BillInquiryQueryHandler :
        IRequestHandler<BillInquiryQuery, SmartPayBillInquiryResponse>
{
    private readonly ISmartPayService _smartPayService;

    public BillInquiryQueryHandler(ISmartPayService smartPayService)
    {
        _smartPayService = smartPayService;
    }

    public async Task<SmartPayBillInquiryResponse> Handle(
        BillInquiryQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ConsumerNo))
            throw new ArgumentException("Consumer number is required.");

        return await _smartPayService.BillInquiryAsync(
            request.ConsumerNo,
            cancellationToken);
    }
}

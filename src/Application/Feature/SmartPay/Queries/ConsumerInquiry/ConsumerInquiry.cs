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
    private readonly ISmartPayService _smartPayService;

    public ConsumerInquiryQueryHandler(ISmartPayService smartPayService)
    {
        _smartPayService = smartPayService;
    }

    public async Task<SmartPayConsumerInquiryResponse> Handle(
        ConsumerInquiryQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CellNo))
            throw new ArgumentException("Cell number is required.");

        // Call SmartPay API
        return await _smartPayService.ConsumerInquiryAsync(
            request.CellNo,
            cancellationToken);
    }
}

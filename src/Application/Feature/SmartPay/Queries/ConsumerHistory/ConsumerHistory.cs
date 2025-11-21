using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.ConsumerHistory;
public class ConsumerHistoryQuery : IRequest<SmartPayConsumerHistoryResponse>
{
    public string ConsumerNo { get; set; } = default!;
}
public class ConsumerHistoryQueryHandler :
        IRequestHandler<ConsumerHistoryQuery, SmartPayConsumerHistoryResponse>
{
    private readonly ISmartPayService _smartPayService;

    public ConsumerHistoryQueryHandler(ISmartPayService smartPayService)
    {
        _smartPayService = smartPayService;
    }

    public async Task<SmartPayConsumerHistoryResponse> Handle(
        ConsumerHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ConsumerNo))
            throw new ArgumentException("Consumer number is required.");

        return await _smartPayService.ConsumerHistoryAsync(
            request.ConsumerNo,
            cancellationToken);
    }
}

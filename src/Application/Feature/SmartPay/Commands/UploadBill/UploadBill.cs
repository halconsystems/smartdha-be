using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.SmartPay.Commands.UploadBill;
public class UploadBillCommand : IRequest<SmartPayUploadBillResponse>
{
    public SmartPayUploadBillRequest Request { get; set; } = default!;
}
public class UploadBillCommandHandler :
        IRequestHandler<UploadBillCommand, SmartPayUploadBillResponse>
{
    private readonly ISmartPayService _smartPayService;

    public UploadBillCommandHandler(ISmartPayService smartPayService)
    {
        _smartPayService = smartPayService;
    }

    public async Task<SmartPayUploadBillResponse> Handle(
        UploadBillCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Request == null)
            throw new ArgumentException("Upload bill request body is required.");

        return await _smartPayService.UploadBillAsync(
            request.Request,
            cancellationToken);
    }
}

using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSVoucher.Commands.CreatePayment;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSVoucher.Commands.GenerateVoucher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/pms/finance")]
public class FinanceController : ControllerBase
{
    private readonly IMediator _mediator;
    public FinanceController(IMediator mediator) => _mediator = mediator;

    [HttpPost("vouchers/generate")]
    public async Task<IActionResult> GenerateVoucher(GenerateVoucherCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("payments")]
    public async Task<IActionResult> Payment(CreatePaymentCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));
}


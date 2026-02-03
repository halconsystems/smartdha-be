using DHAFacilitationAPIs.Application.Feature.PaymentBills.Commands.AddPayLateFeePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "payment")]
public class PaymentController : BaseApiController
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/payment/late-fee-policy")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePayLateFeePolicyCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}

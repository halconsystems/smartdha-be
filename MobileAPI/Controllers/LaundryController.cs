using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Laundry")]
public class LaundryController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryController(IMediator med) => _mediator = med;


    [HttpPost("Create-Order"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrderPlaceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));
}

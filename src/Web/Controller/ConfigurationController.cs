using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DHAFacilitationAPIs.Application.Feature.Configuration.Commands;
using Microsoft.AspNetCore.Authorization;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class ConfigruationController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    public ConfigruationController(IUser loggedInUser, IMediator mediator)
    {
        _loggedInUser = loggedInUser;
        _mediator = mediator;
    }

    [HttpPut("bowser-orders-config"), AllowAnonymous]
    public async Task<IActionResult> UpdateBowserOrders([FromBody] UpdateBowserOrderConfigCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}




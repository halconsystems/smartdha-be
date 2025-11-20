using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Configuration.Commands;
using DHAFacilitationAPIs.Application.Feature.Configuration.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("get-bowser-orders-config"), AllowAnonymous]
    public async Task<IActionResult> GetBowserOrdersConfig()
    {
        var result = await _mediator.Send(new GetBowserOrdersConfigQuery());
        return Ok(result);
    }

    [HttpPut("update-bowser-orders-config"), AllowAnonymous]
    public async Task<IActionResult> UpdateBowserOrders([FromBody] UpdateBowserOrderConfigCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}




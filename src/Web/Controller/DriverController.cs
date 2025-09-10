using DHAFacilitationAPIs.Application.Feature.Drivers.Commands;
using DHAFacilitationAPIs.Application.Feature.Drivers.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Web.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]

public class DriverController : BaseApiController
{
    private readonly IMediator _mediator;
    public DriverController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Add")]
    public async Task<ActionResult<SuccessResponse<string>>> Add(AddDriverCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("Update")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(UpdateDriverCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteDriverCommand(id)));

    [HttpGet("Get")]
    public async Task<ActionResult<SuccessResponse<List<DriverInfoDto>>>> Get([FromQuery] Guid? id)
        => Ok(await _mediator.Send(new GetDriversQuery(id)));
}


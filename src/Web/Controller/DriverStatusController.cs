using DHAFacilitationAPIs.Application.Feature.DriverStatus.Commands;
using DHAFacilitationAPIs.Application.Feature.DriverStatus.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Web.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]

public class DriverStatusController : BaseApiController
{
    private readonly IMediator _mediator;
    public DriverStatusController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Add")]
    public async Task<ActionResult<SuccessResponse<string>>> Add(AddDriverStatusCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("Update")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(UpdateDriverStatusCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteDriverStatusCommand(id)));

    [HttpGet("Get")]
    public async Task<ActionResult<SuccessResponse<List<DriverStatusDto>>>> Get([FromQuery] Guid? id)
        => Ok(await _mediator.Send(new GetDriverStatusQuery(id)));
}


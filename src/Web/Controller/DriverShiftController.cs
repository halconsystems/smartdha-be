using DHAFacilitationAPIs.Application.Feature.DriverShift.Commands;
using DHAFacilitationAPIs.Application.Feature.DriverShift.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class DriverShiftController : ControllerBase
{
    private readonly IMediator _mediator;
    public DriverShiftController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Add")]
    public async Task<ActionResult<SuccessResponse<string>>> Add(AddDriverShiftCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("Update")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(UpdateDriverShiftCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteDriverShiftCommand(id)));

    [HttpGet("Get")]
    public async Task<ActionResult<SuccessResponse<List<DriverShiftDto>>>> Get([FromQuery] Guid? id)
        => Ok(await _mediator.Send(new GetDriverShiftQuery(id)));
}

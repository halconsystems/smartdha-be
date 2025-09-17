using DHAFacilitationAPIs.Application.Feature.Shifts.Commands;
using DHAFacilitationAPIs.Application.Feature.Shifts.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class ShiftController : ControllerBase
{

    private readonly IMediator _mediator;
    public ShiftController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Add")]
    public async Task<ActionResult<SuccessResponse<string>>> Add(AddShiftCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("Update")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(UpdateShiftCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteShiftCommand(id)));

    [HttpGet("Get")]
    public async Task<ActionResult<SuccessResponse<List<ShiftDto>>>> Get([FromQuery] Guid? id)
        => Ok(await _mediator.Send(new GetShiftQuery(id)));
}


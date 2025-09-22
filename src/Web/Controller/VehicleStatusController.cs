using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
using DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class VehicleStatusController : BaseApiController
{
    private readonly IMediator _mediator;
    public VehicleStatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Add"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddVehicleStatus([FromBody] AddVehicleStatusCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }


    [HttpPut("Update"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> UpdateVehicleStatus([FromBody] UpdateVehicleStatusCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("Delete/{id}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteVehicleStatusCommand(id));
        return Ok(result);
    }

    [HttpGet("Get"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<object>>> Get([FromQuery] Guid? id)
    {
        var result = await _mediator.Send(new GetVehicleStatusQuery(id));
        return Ok(result);
    }
}

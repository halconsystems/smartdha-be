using DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.AssignDriverToBowser;
using DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.UpdateDriverAssignment;
using DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAssignedDrivers;
using DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAvailableDriverVehicles;
using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserStatuses.Commands;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class BowserAssignmentController : BaseApiController
{
    private readonly IMediator _mediator;

    public BowserAssignmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Get-Available Driver Vehicles"), AllowAnonymous]
    public async Task<ActionResult<List<AvailableDriverVehicleDto>>> GetAvailableDriverVehicles(
        [FromQuery] DateTime requestedDeliveryDate,
        [FromQuery] Guid phaseId,
        [FromQuery] Guid bowserCapacityId)
    {
        var result = await _mediator.Send(new GetAvailableDriverVehiclesQuery(requestedDeliveryDate, phaseId, bowserCapacityId));
        return Ok(result);
    }

    [HttpPut("assign-driver"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriver([FromBody] AssignDriverToBowserDto dto, CancellationToken ct)
    {
        if (dto == null)
            return BadRequest("Request body is missing or invalid");
        var response = await _mediator.Send(new AssignDriverToBowserCommand(dto), ct);
        return Ok(response);
    }

    [HttpPut("update-driver-assignment"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateDriverAssignment([FromBody] UpdateDriverAssignmentDto dto, CancellationToken ct)
    {
        var response = await _mediator.Send(new UpdateDriverAssignmentCommand(dto), ct);
        return Ok(response);
    }

    [HttpGet("get-assigned-drivers"), AllowAnonymous]
    public async Task<ActionResult<List<AssignedDriverDto>>> GetAssignedDrivers(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAssignedDriversQuery(), ct);
        return Ok(new SuccessResponse<List<AssignedDriverDto>>(result, "Assigned drivers fetched successfully."));
    }

    [HttpPut("update-bowser-status"), AllowAnonymous]
    public async Task<IActionResult> UpdateBowserStatus([FromBody] UpdateBowserStatusCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(new { message = result });
    }
}

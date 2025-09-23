using DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.AssignDriverToBowser;
using DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAvailableDriverVehicles;
using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
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

    [HttpPost("assign-driver"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriver([FromBody] AssignDriverToBowserDto dto)
    {
        var response = await _mediator.Send(new AssignDriverToBowserCommand(dto));
        return Ok(response);
    }
}

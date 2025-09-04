using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
using DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.Assignment;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class BowserController : BaseApiController
{
    private readonly IMediator _mediator;

    public BowserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("AddBowserCapacity"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddBowserCapacity([FromBody] AddBowserCapacityCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("UpdateBowserCapacity"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> UpdateBowserCapacity([FromBody] UpdateBowserCapacityCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("{id}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBowserCapacityCommand { Id = id });
        return Ok(result);
    }

    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<object>>> Get([FromQuery] Guid? id)
    {
        var result = await _mediator.Send(new GetBowserCapacityQuery { Id = id });
        return Ok(result);
    }






}

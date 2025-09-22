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
public class BowserMakeController : BaseApiController
{
    private readonly IMediator _mediator;
    public BowserMakeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Add"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddBowserMake([FromBody] AddBowserMakeCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }


    [HttpPut("Update"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> UpdateBowserMake([FromBody] UpdateBowserMakeCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("Delete/{id}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
    {
        var result = await _mediator.Send( new DeleteBowserMakeCommand (id ));
        return Ok(result);
    }

    [HttpGet("Get"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<object>>> Get([FromQuery] Guid? id)
    {
        var result = await _mediator.Send(new GetBowserMakesQuery (id ));
        return Ok(result);
    }
}

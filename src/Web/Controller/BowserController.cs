using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Commands;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries;
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

    //Bowser 
    [HttpPost("Add"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddBowser([FromBody] AddBowserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }


    [HttpPut("Update"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> UpdateBowser([FromBody] UpdateBowserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("Delete/{id}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBowserCommand ( id) );
        return Ok(result);
    }

    [HttpGet("Get"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<BowserDto>>>> Get([FromQuery] Guid? id)
    {
        var result = await _mediator.Send(new GetBowsersQuery ( id ));
        return Ok(result);
    }
}

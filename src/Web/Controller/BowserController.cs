using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
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

}

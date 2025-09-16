using DHAFacilitationAPIs.Application.Feature.BowserOwner.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserOwner.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class BowserOwnerController : BaseApiController
{
    private readonly IMediator _mediator;
    public BowserOwnerController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Add")]
    public async Task<ActionResult<SuccessResponse<string>>> Add(AddBowserOwnerCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("Update")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(UpdateBowserOwnerCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteBowserOwnerCommand(id)));

    [HttpGet("Get")]
    public async Task<ActionResult<SuccessResponse<List<BowserOwnerDto>>>> Get([FromQuery] Guid? id)
        => Ok(await _mediator.Send(new GetBowserOwnerQuery(id)));

}

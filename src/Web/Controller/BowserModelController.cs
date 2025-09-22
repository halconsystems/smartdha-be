using DHAFacilitationAPIs.Application.Feature.BowserModel.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserModel.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class BowserModelController : BaseApiController
{
    private readonly IMediator _mediator;
    public BowserModelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Add"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Add(AddBowserModelCommand command)
      => Ok(await _mediator.Send(command));

    [HttpPut("Update"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Update(UpdateBowserModelCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("Delete/{id}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteBowserModelCommand(id)));

    [HttpGet("Get"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<BowserModelDto>>>> Get([FromQuery] Guid? id)
        => Ok(await _mediator.Send(new GetBowserModelsQuery(id)));
}


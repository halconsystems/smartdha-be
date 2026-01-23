using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class LaundryServicesController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryServicesController(IMediator med) => _mediator = med;

    [HttpPost("Create-LaundryServices"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateAddLaundryServiceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Modify-LaundryService-{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, ModifyLaundryServiceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryServices"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryServices(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryServicesQuery(), ct);
        return Ok(result);
    }

    [HttpDelete("Delete-LaundryServices"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteLaundryServiceCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryServices")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, bool Active, ActiveInActiveLaundryServiceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));
}

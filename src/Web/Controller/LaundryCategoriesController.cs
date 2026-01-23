using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.DeleteRoom;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class LaundryCategoriesController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryCategoriesController(IMediator med) => _mediator = med;
    [HttpPost("Create-LaundryCategory"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateLaundryCategory(CreateLaundryCategorCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-LaundryCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, ModifyLaundryCategoryCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryCategory"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryCategory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryCategoryQuery(), ct);
        return Ok(result);
    }
    [HttpDelete("Delete-LaundryCategory"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteLaundryCategoryCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, bool Active, ActiveInActiveLaundryCategoryCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

}

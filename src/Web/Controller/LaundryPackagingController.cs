using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class LaundryPackagingController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryPackagingController(IMediator med) => _mediator = med;
    [HttpPost("Create-LaundryPackaging"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateLaundryPackaging(CreateLaundryPackagingCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("UpdateCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryPackaging(Guid id, ModifyLaundryPackagingCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryPackaging"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryPackaging(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryPackagingQuery(), ct);
        return Ok(result);
    }
}

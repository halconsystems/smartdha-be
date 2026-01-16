using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;


[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class LaundryItemsController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryItemsController(IMediator med) => _mediator = med;
    [HttpPost("Create-LaundryItems"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateLaundryItems(CreateLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-LaundryItems")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryItems(Guid id, ModifyLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryItems"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryItemsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryItems-ById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSectById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLaundryItemByIdQuery(CategoryId), ct);
        return Ok(result);
    }
}

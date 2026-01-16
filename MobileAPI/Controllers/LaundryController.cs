using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Laundry")]
public class LaundryController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryController(IMediator med) => _mediator = med;


    [HttpPost("Create-Order")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrderPlaceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("get-LaundryItems")]
    public async Task<ActionResult<LaundryItemsDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryItemsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryItems-ById")]
    public async Task<ActionResult<LaundryItemsDTO>> GetLaundryItemById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLaundryItemByIdQuery(CategoryId), ct);
        return Ok(result);
    }


    [HttpGet("get-LaundryItemsAfterHangerPrice")]
    public async Task<ActionResult<LaundryItemsDTO>> GetLaundryItemsByPackage(Guid packageID, CancellationToken ct)
    {
        var result = await _mediator.Send(new HangerPriceAdjustmentQuery(packageID), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryServices")]
    public async Task<ActionResult<LaundryServiceDTO>> GetAllLaundryServices(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryServicesQuery(), ct);
        return Ok(result);
    }
    [HttpGet("get-LaundryPackaging")]
    public async Task<ActionResult<LaundryPackagingDTO>> GetAllLaundryPackaging(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryPackagingQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryCategory")]
    public async Task<ActionResult<LaundryCategoryDTO>> GetAllLaundryCategory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryCategoryQuery(), ct);
        return Ok(result);
    }
}

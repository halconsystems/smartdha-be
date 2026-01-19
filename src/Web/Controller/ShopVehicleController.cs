using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.Feature.Shops.Queries;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Laundry")]
public class ShopVehicleController : BaseApiController
{
    private readonly IMediator _mediator;
    public ShopVehicleController(IMediator med) => _mediator = med;

    [HttpPost("Create-ShopVehicle")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(CreateShopVehicleCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("get-ShopVehicle")]
    public async Task<ActionResult<ShopVehicleDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopVehiclesQuery(), ct);
        return Ok(result);
    }
    [HttpGet("get-Shops")]
    public async Task<ActionResult<ShopQueryDTO>> GetShops(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllShopQueryQuery(), ct);
        return Ok(result);
    }

}

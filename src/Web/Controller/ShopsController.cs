using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.Shops.Command;
using DHAFacilitationAPIs.Application.Feature.Shops.Queries;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class ShopsController : BaseApiController
{
    private readonly IMediator _mediator;
    public ShopsController(IMediator med) => _mediator = med;
    [HttpPost("Create-Shops"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateShops(CreateShopsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-Shops")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateShops(Guid id, UpdateShopCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { ShopId = id }, ct));

    [HttpGet("get-AllShops"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllShopQueryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-Shop-ById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSectById(Guid ShopId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopByIdQuery(ShopId), ct);
        return Ok(result);
    }

    [HttpPost("Assign-Driver-Shops"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriversToShops(AssignDriverToShopCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Assign-Driver-Vehicle"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriversToVehicle(AssignedDriverToVehiclesCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));
}

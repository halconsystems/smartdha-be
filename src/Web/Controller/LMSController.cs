using DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Command;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Command;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.Delivery;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Command;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicRequests;
using DHAFacilitationAPIs.Application.Feature.Religion.Command;
using DHAFacilitationAPIs.Application.Feature.Religion.Queries;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Command;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DHAFacilitationAPIs.Application.Feature.Orders.Queries.OrderDashBoardDTO;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class LMSController : BaseApiController
{
    private readonly IMediator _mediator;
    public LMSController(IMediator med) => _mediator = med;

    [HttpPost("Create-Discount-Tax"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(CreateOrderDiscountCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));
    [HttpPost("Order-Disptach"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(AssignOrderDispatchCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));
    [HttpPost("Order-Process"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrdersProcessAtShopCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    //[HttpPost("Create-Discount-Tax"), AllowAnonymous]
    //public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(CreateOrderDiscountCommand cmd, CancellationToken ct)
    //   => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetOrder-Dsicount-Tax"), AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderDTSetting(), ct);
        return Ok(result);
    }
    [HttpGet("GetOrder-List"), AllowAnonymous]
    public async Task<IActionResult> GetOrderList(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderListQuery(), ct);
        return Ok(result);
    }
    [HttpGet("GetOrderHistory"), AllowAnonymous]
    public async Task<IActionResult> GetAll(Guid Id,CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderHistoryIdQuery(Id), ct);
        return Ok(result);
    }
    [HttpGet("Dashboard")]
    public Task<OrderDashboardSummaryDto> Dashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => _mediator.Send(new GetOrderDashboardSummaryQuery(from, to));

}

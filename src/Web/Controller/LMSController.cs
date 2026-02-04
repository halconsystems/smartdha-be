using System.Text.Json;
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
using DHAFacilitationAPIs.Application.Feature.OrderPaymentIpn.Command;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Command;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.RegisterDriver;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicRequests;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
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


    [HttpPost("Create-Order")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrderPlaceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));


    [HttpGet("get-LaundryItemsAfterHangerPrice")]
    public async Task<ActionResult<LaundryItemsDTO>> GetLaundryItemsByPackage(Guid packageID, CancellationToken ct)
    {
        var result = await _mediator.Send(new HangerPriceAdjustmentQuery(packageID), ct);
        return Ok(result);
    }

    [HttpPost("checkout")]
    [AllowAnonymous] // IPN usually unauthenticated
    public async Task<IActionResult> ReceiveIpn(CancellationToken ct)
    {
        PaymentIpnRequestDto dto;
        string rawPayload;

        if (Request.HasFormContentType)
        {
            var form = Request.Form;
            dto = new PaymentIpnRequestDto
            {
                err_code = form["err_code"],
                err_msg = form["err_msg"],
                basket_id = form["basket_id"],
                transaction_id = form["transaction_id"],
                responseKey = form["responseKey"],
                Response_Key = form["Response_Key"],
                validation_hash = form["validation_hash"],
                order_date = form["order_date"],
                amount = form["amount"],
                transaction_amount = form["transaction_amount"],
                merchant_amount = form["merchant_amount"],
                discounted_amount = form["discounted_amount"],
                transaction_currency = form["transaction_currency"],
                PaymentName = form["PaymentName"],
                issuer_name = form["issuer_name"],
                masked_pan = form["masked_pan"],
                mobile_no = form["mobile_no"],
                email_address = form["email_address"],
                is_international = form["is_international"],
                recurring_txn = form["recurring_txn"],
                bill_number = form["bill_number"],
                customer_id = form["customer_id"],
                rdv_message_key = form["rdv_message_key"],
                additional_value = form["additional_value"]
            };

            rawPayload = JsonSerializer.Serialize(
                form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }
        else
        {
            using var reader = new StreamReader(Request.Body);
            rawPayload = await reader.ReadToEndAsync(ct);

            if (string.IsNullOrWhiteSpace(rawPayload))
                throw new InvalidOperationException("Empty JSON payload.");

            dto = JsonSerializer.Deserialize<PaymentIpnRequestDto>(
                rawPayload,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }

        await _mediator.Send(
            new SaveOrderPaymentIpnCommand(dto, rawPayload),
            ct);
        if (dto.err_code == "000")
        {
            return Ok(new { status = "Success", message = dto.err_msg });
        }
        else
        {
            return Ok(new { status = "Failed", message = dto.err_msg });
        }
    }

    [HttpGet("get-OrderList")]
    public async Task<ActionResult<LaundryCategoryDTO>> GetAllOrder(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderListQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-OrderHsitory-ById")]
    public async Task<ActionResult<OrderHistoryDTO>> GetOrderHisotryById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderHistoryIdQuery(CategoryId), ct);
        return Ok(result);
    }


    [HttpPost("Order/Order-Dispatch")]
    public async Task<ActionResult<string>> AcceptDispatch(AssignOrderDispatchCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }

    [HttpPost("Order/RidersTask")]
    public async Task<ActionResult<string>> AcceptDispatch(RiderPickupOrderDispatch cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }
    [HttpPost("Order/ShopProcess")]
    public async Task<ActionResult<string>> ShopProcess(OrdersProcessAtShopCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }


}

using System.Text.Json;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.Delivery;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;
using DHAFacilitationAPIs.Application.Feature.OrderPaymentIpn.Command;
using DHAFacilitationAPIs.Application.Feature.Orders.Command;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AcceptPanicDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverLogin;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyAssignedPanic;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
using DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;
using DHAFacilitationAPIs.Application.Feature.Shops.Queries;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Laundry")]
public class LaundryController : BaseApiController
{
    private readonly IMediator _mediator;
    public LaundryController(IMediator med) => _mediator = med;

    #region Laundry Orders Creation Here
    [HttpGet("get-LaundryItems")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryItemsDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryItemsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryItems-ById")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryItemsDTO>> GetLaundryItemById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLaundryItemByIdQuery(CategoryId), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryItemsAfterHangerPrice")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryItemsDTO>> GetLaundryItemsByPackage(Guid packageID, CancellationToken ct)
    {
        var result = await _mediator.Send(new HangerPriceAdjustmentQuery(packageID), ct);
        return Ok(result);
    }
    [HttpGet("get-LaundryServices")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryServiceDTO>> GetAllLaundryServices(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryServicesQuery(), ct);
        return Ok(result);
    }
    [HttpGet("get-LaundryPackaging")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryPackagingDTO>> GetAllLaundryPackaging(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryPackagingQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryCategory")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryCategoryDTO>> GetAllLaundryCategory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryCategoryQuery(), ct);
        return Ok(result);
    }
    [HttpPost("Create-Order")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrderPlaceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("get-AllShops")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryCategoryDTO>> GetAllShops(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllShopQueryQuery(), ct);
        return Ok(result);
    }

    [HttpPost("checkout")]
    [Tags("01 - Laundry-Orders")]
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
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<LaundryCategoryDTO>> GetAllOrder(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderListQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-OrderHsitory-ById")]
    [Tags("01 - Laundry-Orders")]
    public async Task<ActionResult<OrderHistoryDTO>> GetOrderHisotryById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderHistoryIdQuery(CategoryId), ct);
        return Ok(result);
    }

    [HttpGet("GetOrder-Dsicount-Tax"), AllowAnonymous]
    [Tags("01 - Laundry-Orders")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderDTSetting(), ct);
        return Ok(result);
    }
    #endregion


    #region Laundry Driver Process


    //[HttpPost("Drivers/AssignDriverToVehicle")]
    //[Tags("02 - Laundry-Drivers")]
    //public async Task<ActionResult> AssignVehicle(AssignedDriverToVehiclesCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    return Ok(result);
    //}

    [HttpPost("Order/RidersTask")]
    [Tags("02 - Laundry-Drivers")]
    public async Task<ActionResult<string>> AcceptDispatch(RiderPickupOrderDispatch cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }

    [HttpPost("Assign-Driver-Vehicle"), AllowAnonymous]
    [Tags("02 - Laundry-Drivers")]
    public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriversToVehicle(AssignedDriverToVehiclesCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("get-ShopVehicle")]
    [Tags("02 - Laundry-Drivers")]
    public async Task<ActionResult<ShopVehicleDTO>> GetShopVehicle(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopVehiclesforDriverQuery(), ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("Driver/Login")]
    [Tags("02 - Laundry-Drivers")]
    public async Task<ActionResult> LoginDriver(DriverLoginCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Driver/Order/MyAssigned")]
    [Tags("02 - Laundry-Drivers")]
    public async Task<ActionResult<PanicUpdatedRealtimeDto>> GetMyAssignedPanic(bool Pickup)
    {
        var result = await _mediator.Send(new GetMyAssignedOrderQuery(Pickup));
        return Ok(result);
    }
    #endregion



    //[HttpPost("Drivers/AssignDriverToShop")]
    //public async Task<ActionResult> AssignVehicle(AssignDriverToShopCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    return Ok(result);
    //}


    //[HttpPost("Order/Order-Dispatch")]
    //public async Task<ActionResult<string>> AcceptDispatch(AssignOrderDispatchCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    return Ok(result);
    //}


    //[HttpPost("Order/ShopProcess")]
    //public async Task<ActionResult<string>> ShopProcess(OrdersProcessAtShopCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    return Ok(result);
    //}
}

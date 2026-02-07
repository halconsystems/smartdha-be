using System.Text.Json;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryShopDT;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;
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
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllDrivers;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicRequests;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetDriverById;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
using DHAFacilitationAPIs.Application.Feature.Religion.Command;
using DHAFacilitationAPIs.Application.Feature.Religion.Queries;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Command;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;
using DHAFacilitationAPIs.Application.Feature.ShopDriver.Command;
using DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;
using DHAFacilitationAPIs.Application.Feature.Shops.Command;
using DHAFacilitationAPIs.Application.Feature.Shops.Queries;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Queries;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateDriverInfo;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
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
    private readonly IFileStorageService _files;
    public LMSController(IMediator med,IFileStorageService fileStorage)
    {
        _mediator = med;
        _files = fileStorage;

    }

    #region Laundry Category Crud Here
    [HttpPost("Create-LaundryCategory"), AllowAnonymous]
    [Tags("01 - LaundryCategory")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateLaundryCategory(CreateLaundryCategorCommand cmd, CancellationToken ct)
     => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-LaundryCategory")]
    [Tags("01 - LaundryCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, ModifyLaundryCategoryCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryCategory"), AllowAnonymous]
    [Tags("01 - LaundryCategory")]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryCategory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryCategoryQuery(), ct);
        return Ok(result);
    }
    [HttpDelete("Delete-LaundryCategory"), AllowAnonymous]
    [Tags("01 - LaundryCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteLaundryCategoryCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryCategory")]
    [Tags("01 - LaundryCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, bool Active, ActiveInActiveLaundryCategoryCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));
    #endregion

    #region Laundry Item Crud Here
    [HttpPost("Create-LaundryItems"), AllowAnonymous]
    [Tags("02 - LaundryItems")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateLaundryItems(CreateLaundryItemsCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-LaundryItems")]
    [Tags("02 - LaundryItems")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryItems(Guid id, ModifyLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryItems"), AllowAnonymous]
    [Tags("02 - LaundryItems")]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryItemsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryItems-ById"), AllowAnonymous]
    [Tags("02 - LaundryItems")]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSectById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLaundryItemByIdQuery(CategoryId), ct);
        return Ok(result);
    }

    [HttpDelete("Delete-LaundryItems"), AllowAnonymous]
    [Tags("02 - LaundryItems")]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteLaundryItemsCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryItems")]
    [Tags("02 - LaundryItems")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, bool Active, ActiveInActiveLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Laundry/images/add"), AllowAnonymous]
    [Tags("02 - LaundryItems")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddLaundryImage(
        Guid LaundryId,
        [FromForm] AddLaundryItemForm form,
        CancellationToken ct
        )
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (form.Files == null)
            return BadRequest("No images uploaded.");

        // Keep parallel arrays aligned (if provided)
        if (form.ImageNames == null)
            return BadRequest("ImageNames count must match Files count.");
        if (form.Descriptions == null)
            return BadRequest("Descriptions count must match Files count.");

        var folder = $"LaundryItems/{LaundryId}";

        var file = form.Files;

        // Save file (your IFileStorageService validates size/ext & creates folder)
        //var relativePath = await _files.SaveFileAsync(file, folder, ct);
        var relativePath = await _files.SaveFileAsync(
          file,
          folder,
          ct,
          maxBytes: 5 * 1024 * 1024,                       // 5 MB
          allowedExtensions: new[] { ".jpg", ".jpeg", ".png" }
         );

        var ext = Path.GetExtension(relativePath);

        // Pick metadata by index (or defaults)
        var name = form.ImageNames;
        var desc = form.Descriptions;
        var cat = form.Categories;

        var uploaded = new AddLaundryImageDTO(
        ImageURL: relativePath,
        ImageExtension: ext,
        ImageName: name,
        Description: desc,
        Category: ImageCategory.Main
    );

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddLaundryImagesCommand(LaundryId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }
    #endregion

    #region Laundry Services Crud Here
    [HttpPost("Create-LaundryServices"), AllowAnonymous]
    [Tags("03 - LaundryServices")]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateAddLaundryServiceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Modify-LaundryService-{id:guid}")]
    [Tags("03 - LaundryServices")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, ModifyLaundryServiceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryServices"), AllowAnonymous]
    [Tags("03 - LaundryServices")]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryServices(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryServicesQuery(), ct);
        return Ok(result);
    }

    [HttpDelete("Delete-LaundryServices"), AllowAnonymous]
    [Tags("03 - LaundryServices")]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteLaundryServiceCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryServices")]
    [Tags("03 - LaundryServices")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id, bool Active, ActiveInActiveLaundryServiceCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));
    #endregion

    


    #region Laundry Tax-Dicount Crud Here
    [HttpPost("Create-Discount-Tax"), AllowAnonymous]
    [Tags("04 - Laundry-Discount-Tax")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(CreateOrderDiscountCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetOrder-Dsicount-Tax"), AllowAnonymous]
    [Tags("04 - Laundry-Discount-Tax")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderDTSetting(), ct);
        return Ok(result);
    }
    #endregion

    #region Laundry Process Here
    [HttpPost("Order-Disptach"), AllowAnonymous]
    [Tags("05 - OrderProcessFromWeb")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(AssignOrderDispatchCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Order-Disptach-Bulk")]
    [Tags("05 - OrderProcessFromWeb")]
    public async Task<ActionResult<SuccessResponse<List<DisptachDTO>>>> CreateOrder(AssignBulkOrderDispatchCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Order-Process"), AllowAnonymous]
    [Tags("05 - OrderProcessFromWeb")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrdersProcessAtShopCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetOrder-List"), AllowAnonymous]
    [Tags("05 - OrderProcessFromWeb")]
    public async Task<IActionResult> GetOrderList(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderListQuery(), ct);
        return Ok(result);
    }
    [HttpGet("GetOrderHistory"), AllowAnonymous]
    [Tags("05 - OrderProcessFromWeb")]
    public async Task<IActionResult> GetAll(Guid Id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderHistoryIdQuery(Id), ct);
        return Ok(result);
    }

    [HttpGet("Dashboard")]
    [Tags("05 - OrderProcessFromWeb")]
    public Task<OrderDashboardSummaryDto> Dashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => _mediator.Send(new GetOrderDashboardSummaryQuery(from, to));
    #endregion

    #region Laundry Orders From Web Here

    [HttpPost("Create-Order")]
    [Tags("06 - LaundryOrder-From-Web")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(OrderPlaceCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));


    [HttpGet("get-LaundryItems-After-Hanger-Adjustment")]
    [Tags("06 - LaundryOrder-From-Web")]
    public async Task<ActionResult<LaundryItemsDTO>> GetLaundryItemsByPackage(Guid packageID, CancellationToken ct)
    {
        var result = await _mediator.Send(new HangerPriceAdjustmentQuery(packageID), ct);
        return Ok(result);
    }

    [HttpPost("checkout")]
    [Tags("06 - LaundryOrder-From-Web")]
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
    [Tags("06 - LaundryOrder-From-Web")]
    public async Task<ActionResult<LaundryCategoryDTO>> GetAllOrder(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllOrderListQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-OrderHsitory-ById")]
    [Tags("06 - LaundryOrder-From-Web")]
    public async Task<ActionResult<OrderHistoryDTO>> GetOrderHisotryById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderHistoryIdQuery(CategoryId), ct);
        return Ok(result);
    }
    #endregion

    #region Laundry Mobile Process Here

    [HttpPost("Order/RidersTask")]
    [Tags("07 - Laundry-Mobile-Task")]
    public async Task<ActionResult<string>> AcceptDispatch(RiderPickupOrderDispatch cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }
    #endregion

    #region Laundry Shop Crud Here
    [HttpPost("Create-Shops"), AllowAnonymous]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateShops(CreateShopsCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-Shops")]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateShops(Guid id, UpdateShopCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { ShopId = id }, ct));

    [HttpDelete("Delete-Shops"), AllowAnonymous]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteShop([FromQuery] DeleteFMShopsCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("get-AllShops"), AllowAnonymous]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult<MemberShipDTO>> GetAllShops(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllShopQueryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-Shop-ById"), AllowAnonymous]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult<MemberShipDTO>> GetAllShopById(Guid ShopId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopByIdQuery(ShopId), ct);
        return Ok(result);
    }

    [HttpPost("Assign-Driver-Shops"), AllowAnonymous]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriversToShops(AssignDriverToShopCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetAll-Shop-Drivers")]
    [Tags("08 - Laundry-Shop")]
    public async Task<IActionResult> GetDrivers()
    {
        var result = await _mediator.Send(new GetAllShopDriversQuery());
        return Ok(result);
    }

    [HttpGet("Shop-Drivers/{id:guid}")]
    [Tags("08 - Laundry-Shop")]
    public async Task<IActionResult> GetDriver(Guid id)
    {
        var result = await _mediator.Send(new GetShopDriverByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("Shop-Drivers/{id:guid}")]
    [Tags("08 - Laundry-Shop")]
    public async Task<IActionResult> UpdateDriver(Guid id, UpdateShopDriverInfoCommand cmd)
    {
        var updated = await _mediator.Send(cmd with { DriverId = id });
        return Ok(new { message = "Driver updated successfully" });
    }

    [HttpDelete("Shop-Drivers/{id:guid}")]
    [Tags("08 - Laundry-Shop")]
    public async Task<IActionResult> DeleteDriver(Guid id)
    {
        await _mediator.Send(new DeleteShopDriverCommand(id));
        return Ok(new { message = "Driver deleted successfully." });
    }

    //[HttpPost("Assign-Driver-Vehicle"), AllowAnonymous]
    //[Tags("08 - Laundry-Shop")]
    //public async Task<ActionResult<SuccessResponse<Guid>>> AssignDriversToVehicle(AssignedDriverToVehiclesCommand cmd, CancellationToken ct)
    //  => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Drivers/Register")]
    [Tags("08 - Laundry-Shop")]
    public async Task<ActionResult> RegisterDriver(RegisterShopDriverCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }
    #endregion


    #region Laundry Shop Vehicle Crud Here

    [HttpPost("Create-ShopVehicle")]
    [Tags("09 - Laundry-Shop-Vehicle")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateOrder(CreateShopVehicleCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("get-ShopVehicle")]
    [Tags("09 - Laundry-Shop-Vehicle")]
    public async Task<ActionResult<ShopVehicleDTO>> GetShopVehicle(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopVehiclesQuery(), ct);
        return Ok(result);
    }
    #endregion

    #region Laundry Package Crud Here
    [HttpPost("Create-LaundryPackage"), AllowAnonymous]
    [Tags("10 - LaundryPackage")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreatePackage(CreateLaundryPackagingCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Modify-LaundryPackage-{id:guid}")]
    [Tags("10 - LaundryPackage")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdatePackage(Guid id, ModifyLaundryPackagingCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryPackage"), AllowAnonymous]
    [Tags("10 - LaundryPackage")]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryPackage(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryPackagingQuery(), ct);
        return Ok(result);
    }

    [HttpDelete("Delete-LaundryPackage"), AllowAnonymous]
    [Tags("10 - LaundryPackage")]
    public async Task<ActionResult<SuccessResponse<string>>> DeletePackage([FromQuery] DeleteLaundryPackageCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryPackage")]
    [Tags("10 - LaundryPackage")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryPackage(Guid id, bool Active, ActiveInActiveLaundryPackagingCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));
    #endregion


    #region Laundry Shop Discount CRUD Here

    [HttpPost("Create-Shop-Discount-Tax"), AllowAnonymous]
    [Tags("11 - LaundryShopDiscountTax")]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateShopDiscounttax(CreateAddLaundryShopDTCommand cmd, CancellationToken ct)
      => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Modify-Shop-Discount-Tax/{id:guid}")]
    [Tags("11 - LaundryShopDiscountTax")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateShopDiscounttax(Guid id, UpdateLaundryShopDTCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-Shop-Discount-Tax"), AllowAnonymous]
    [Tags("11 - LaundryShopDiscountTax")]
    public async Task<ActionResult<ShopDTdto>> GetAllShopDTDiscount(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllShopDTQuery(), ct);
        return Ok(result);
    }
    [HttpGet("get-Shop-Discount-Tax-Id"), AllowAnonymous]
    [Tags("11 - LaundryShopDiscountTax")]
    public async Task<ActionResult<ShopDTdto>> GetShopDTDiscount(Guid Id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopDTSettingByIdQuery(Id), ct);
        return Ok(result);
    }
    [HttpGet("get-Shop-Discount-Tax-ShopId"), AllowAnonymous]
    [Tags("11 - LaundryShopDiscountTax")]
    public async Task<ActionResult<ShopDTdto>> GetShopDTDiscountbySHopId(Guid ShopId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShopDTByShopIdQuery(ShopId), ct);
        return Ok(result);
    }

    //[HttpDelete("Delete-LaundryPackage"), AllowAnonymous]
    //[Tags("10 - LaundryPackage")]
    //public async Task<ActionResult<SuccessResponse<string>>> DeletePackage([FromQuery] DeleteLaundryPackageCommand command, CancellationToken ct)
    //{
    //    var result = await _mediator.Send(command, ct);
    //    return Ok(result);
    //}

    //[HttpPut("Active-Inactive-LaundryPackage")]
    //[Tags("10 - LaundryPackage")]
    //public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryPackage(Guid id, bool Active, ActiveInActiveLaundryPackagingCommand cmd, CancellationToken ct)
    //   => Ok(await _mediator.Send(cmd with { Id = id }, ct));
    #endregion

    //[HttpPost("Order/Order-Dispatch")]
    //public async Task<ActionResult<string>> AcceptDispatch(AssignOrderDispatchCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    return Ok(result);
    //}

}

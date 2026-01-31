using System.Security.Claims;
using System.Text.Json;
using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.GroundReservations.Command;
using DHAFacilitationAPIs.Application.Feature.GroundReservations.Queries;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderPaymentIpn.Command;
using DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "GroundBooking")]
public class GroundBookingController : BaseApiController
{
    private readonly IMediator _mediator;

    public GroundBookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //[HttpGet("get-user-grounds")]
    //public async Task<IActionResult> GetUserClubs()
    //{
    //    var result = await _mediator.Send(new GetGroundClubsQuery());
    //    return Ok(result);
    //}

    //[HttpGet("search-rooms"), AllowAnonymous]
    //public async Task<IActionResult> SearchRooms([FromQuery] Guid clubId, [FromQuery] DateOnly checkInDate, [FromQuery] DateOnly checkOutDate, [FromQuery] RoomBookingType bookingType)
    //{
    //    var result = await _mediator.Send(new SearchRoomsQuery(clubId, checkInDate, checkOutDate, bookingType));
    //    return Ok(result);
    //}

    //[HttpGet("get-room-details"), AllowAnonymous]
    //public async Task<IActionResult> GetRoomDetails([FromQuery] Guid roomId, [FromQuery] RoomBookingType bookingType)
    //{
    //    var result = await _mediator.Send(new GetRoomDetailsQuery(roomId, bookingType));
    //    return Ok(result);
    //}

    //[HttpPost("create-reservation"), AllowAnonymous]
    //public async Task<ActionResult<Guid>> CreateReservation([FromBody] CreateReservationCommand cmd, CancellationToken ct)
    //{
    //    var reservationId = await _mediator.Send(cmd);
    //    return Ok(reservationId);
    //}

    //[HttpGet("get-all-reservations"), AllowAnonymous]
    //public async Task<ActionResult<List<ReservationListDto>>> GetAllReservations()
    //{
    //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
    //                   ?? User.FindFirst("sub")?.Value;

    //    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
    //    {
    //        return Unauthorized("Invalid or missing user ID in token");
    //    }

    //    var result = await _mediator.Send(new GetAllGroundReservationsQuery(userId));

    //    return Ok(result);
    //}

    //[HttpGet("get-reservation-status"), AllowAnonymous]
    //public async Task<ActionResult<ReservationStatusDto>> GetReservationStatus(Guid reservationId)
    //{
    //    var result = await _mediator.Send(new GetReservationStatusQuery(reservationId));
    //    return Ok(result);
    //}

    [HttpGet("GetGround"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<GroundDTO>>> GetRoom([FromQuery] GetGroundQuery cmd, CancellationToken ct)
     => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetGroundDetail"), AllowAnonymous]
    public async Task<IActionResult> GetRoomDetails([FromQuery] Guid groundID, [FromQuery] GroundCategory GroundCategory, DateOnly bookingDate)
    {
        var result = await _mediator.Send(new GetGroundQueryById(groundID, GroundCategory, bookingDate));
        return Ok(result);
    }

    [HttpGet("GetBookingList"), AllowAnonymous]
    public async Task<ActionResult<List<BookingDTO>>> GetBookingList()
    {
        var result = await _mediator.Send(new GetAllGroundBookingQuery());
        return Ok(result);
    }
    [HttpGet("GetBookingDetails"), AllowAnonymous]
    public async Task<ActionResult<BookingDTO>> GetBookingDetails(Guid BookingId)
    {
        var result = await _mediator.Send(new GetBookingHistoryQuery(BookingId));
        return Ok(result);
    }

    [HttpPost("Ground-Reservation"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(GroundReserveCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

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
            new SaveGroundPaymentIpnCommand(dto, rawPayload),
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

    [HttpGet("search-grounds")]
    public async Task<IActionResult> SearchGround([FromQuery] DateOnly checkInDate, [FromQuery] GroundCategory groundCategory)
    {
        var result = await _mediator.Send(new SearchGroundQuery(checkInDate, groundCategory));
        return Ok(result);
    }
}

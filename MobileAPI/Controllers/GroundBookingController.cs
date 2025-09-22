using System.Security.Claims;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GroundBookingController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroundBookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-user-grounds"), AllowAnonymous]
    public async Task<IActionResult> GetUserClubs()
    {
        var result = await _mediator.Send(new GetGroundClubsQuery());
        return Ok(result);
    }

    [HttpGet("search-rooms"), AllowAnonymous]
    public async Task<IActionResult> SearchRooms([FromQuery] Guid clubId, [FromQuery] DateOnly checkInDate, [FromQuery] DateOnly checkOutDate, [FromQuery] RoomBookingType bookingType)
    {
        var result = await _mediator.Send(new SearchRoomsQuery(clubId, checkInDate, checkOutDate, bookingType));
        return Ok(result);
    }

    [HttpGet("get-room-details"), AllowAnonymous]
    public async Task<IActionResult> GetRoomDetails([FromQuery] Guid roomId, [FromQuery] RoomBookingType bookingType)
    {
        var result = await _mediator.Send(new GetRoomDetailsQuery(roomId, bookingType));
        return Ok(result);
    }

    [HttpPost("create-reservation"), AllowAnonymous]
    public async Task<ActionResult<Guid>> CreateReservation([FromBody] CreateReservationCommand cmd, CancellationToken ct)
    {
        var reservationId = await _mediator.Send(cmd);
        return Ok(reservationId);
    }

    [HttpGet("get-all-reservations"), AllowAnonymous]
    public async Task<ActionResult<List<ReservationListDto>>> GetAllReservations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token");
        }

        var result = await _mediator.Send(new GetAllGroundReservationsQuery(userId));

        return Ok(result);
    }

    [HttpGet("get-reservation-status"), AllowAnonymous]
    public async Task<ActionResult<ReservationStatusDto>> GetReservationStatus(Guid reservationId)
    {
        var result = await _mediator.Send(new GetReservationStatusQuery(reservationId));
        return Ok(result);
    }
}

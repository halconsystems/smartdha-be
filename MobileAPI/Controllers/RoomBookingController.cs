using System.Security.Claims;
using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.CreateRefundRequest;
using DHAFacilitationAPIs.Application.Feature.Refunds.Queries;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ResidenceTypes;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomCategories;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Rooms;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;
using MobileAPI.Controllers;

namespace MobileAPI.Controller;
[ApiController]
[Route("api/[controller]")]
public class RoomBookingController : BaseApiController
{
    private readonly IMediator _mediator;

    public RoomBookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-user-clubs")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<IActionResult> GetUserClubs()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token");
        }

        var result = await _mediator.Send(new GetUserClubsQuery { UserId = userId });

        return Ok(result);
    }

    [HttpGet("search-rooms")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<IActionResult> SearchRooms([FromQuery] Guid clubId, [FromQuery] DateOnly checkInDate, [FromQuery] DateOnly checkOutDate, [FromQuery] RoomBookingType bookingType)
    {
        var result = await _mediator.Send(new SearchRoomsQuery(clubId, checkInDate, checkOutDate, bookingType));
        return Ok(result);
    }

//    [HttpGet("get-available-room-categories"), AllowAnonymous]
//    public async Task<IActionResult> GetAvailableRoomCategories([FromQuery] Guid clubId, [FromQuery] DateTime checkInDate, [FromQuery] DateTime checkOutDate)
//    {
//        var result = await _mediator.Send(new GetAvailableRoomCategoriesQuery(clubId, checkInDate, checkOutDate));
//        return Ok(result);
//    }

//    [HttpGet("get-available-residence-types"), AllowAnonymous]
//    public async Task<IActionResult> GetAvailableResidenceTypes(Guid clubId, Guid roomCategoryId, DateTime checkIn, DateTime checkOut)
//    {
//        var query = new GetAvailableResidenceTypesQuery(clubId, roomCategoryId, checkIn, checkOut);
//        var result = await _mediator.Send(query);
//        return Ok(result);
//    }

//    [HttpGet("get-available-rooms"), AllowAnonymous]
//    public async Task<IActionResult> GetAvailableRooms(
//    [FromQuery] Guid clubId,
//    [FromQuery] Guid roomCategoryId,
//    [FromQuery] Guid residenceTypeId,
//    [FromQuery] DateTime checkInDate,
//    [FromQuery] DateTime checkOutDate,
//    [FromQuery] string bookingType
//)
//    {
//        var result = await _mediator.Send(new GetAvailableRoomsQuery(clubId, roomCategoryId, residenceTypeId, checkInDate, checkOutDate, bookingType));
//        return Ok(result);
//    }

    [HttpGet("get-room-details")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<IActionResult> GetRoomDetails([FromQuery] Guid roomId, [FromQuery] RoomBookingType bookingType)
    {
        var result = await _mediator.Send(new GetRoomDetailsQuery(roomId, bookingType));
        return Ok(result);
    }

    [HttpPost("create-reservation")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<Guid>> CreateReservation([FromBody] CreateReservationCommand cmd, CancellationToken ct)
    {
        var reservationId = await _mediator.Send(cmd);
        return Ok(reservationId);
    }

    [HttpGet("get-all-reservations")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<List<ReservationListDto>>> GetAllReservations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token");
        }

        var result = await _mediator.Send(new GetAllReservationsQuery(userId));

        return Ok(result);
    }

    [HttpGet("get-reservation-status")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<ReservationStatusDto>> GetReservationStatus(Guid reservationId)
    {
        var result = await _mediator.Send(new GetReservationStatusQuery(reservationId));
        return Ok(result);
    }


    [HttpPost("create-refund-request")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<Guid>> CreateRefundRequest([FromBody] CreateRefundRequestCommand cmd, CancellationToken ct)
    {
        var id = await _mediator.Send(cmd, ct);
        return Ok(id);
    }

    [HttpGet("get-refund-request")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<IActionResult> GetMyRefundRequests(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRefundRequestsQuery(), ct);
        return Ok(result);
    }

}

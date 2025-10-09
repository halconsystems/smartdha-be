using System.Security.Claims;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class AdminBookingsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AdminBookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("members/{clubId:guid}")]
    public async Task<IActionResult> GetClubMembers(Guid clubId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClubMembersQuery(clubId), cancellationToken);
        return Ok(new SuccessResponse<List<ClubMemberDto>>(result, "Members fetched successfully."));
    }

    [HttpGet("get-user-grounds"), AllowAnonymous]
    public async Task<IActionResult> GetUserGrounds(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGroundClubsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("search-rooms"), AllowAnonymous]
    public async Task<IActionResult> SearchRooms([FromQuery] Guid clubId, [FromQuery] DateOnly checkInDate, [FromQuery] DateOnly checkOutDate, [FromQuery] RoomBookingType bookingType, CancellationToken ct)
    {
        var result = await _mediator.Send(new SearchRoomsQuery(clubId, checkInDate, checkOutDate, bookingType), ct);
        return Ok(result);
    }

    [HttpGet("get-room-details"), AllowAnonymous]
    public async Task<IActionResult> GetRoomDetails([FromQuery] Guid roomId, [FromQuery] RoomBookingType bookingType, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRoomDetailsQuery(roomId, bookingType), ct);
        return Ok(result);
    }

    [HttpPost("create-reservation"), AllowAnonymous]
    public async Task<ActionResult<Guid>> CreateReservation([FromBody] CreateReservationAdminCommand cmd)
    {
        var reservationId = await _mediator.Send(cmd);
        return Ok(reservationId);
    }

    [HttpGet("get-all-reservations"), AllowAnonymous]
    public async Task<ActionResult<List<ReservationListDto>>> GetAllReservations(CancellationToken ct)
    {
        
        var result = await _mediator.Send(new GetAllGroundReservationsAdminQuery(), ct);

        return Ok(result);
    }

    [HttpGet("get-reservation-status"), AllowAnonymous]
    public async Task<ActionResult<ReservationStatusDto>> GetReservationStatus(Guid reservationId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetReservationStatusQuery(reservationId), ct);
        return Ok(result);
    }
}

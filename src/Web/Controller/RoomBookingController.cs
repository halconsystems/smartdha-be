using System.Security.Claims;
using DHAFacilitationAPIs.Application.Feature.ConfirmBooking.Commands;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.AllReservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationDashboard;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "club")]
public class RoomBookingController : BaseApiController
{
    private readonly IMediator _mediator;

    public RoomBookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-dashboard")]
    public async Task<ActionResult<ReservationDashboardDto>> GetDashboard([FromQuery] ClubType clubType)
    {
        var result = await _mediator.Send(new GetReservationDashboardQuery(clubType));

        return Ok(result);
    }

    [HttpGet("get-all-reservations")]
    public async Task<ActionResult<List<ReservationWebDto>>> GetAllReservations([FromQuery] ClubType clubType)
    {
        var result = await _mediator.Send(new GetAllReservationsuery { ClubType = clubType });

        return Ok(result);
    }

    [HttpPost("confirm-payment")]
    public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingCommand command, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}

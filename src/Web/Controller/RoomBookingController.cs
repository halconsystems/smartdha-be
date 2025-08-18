using System.Security.Claims;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.AllReservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationDashboard;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class RoomBookingController : BaseApiController
{
    private readonly IMediator _mediator;

    public RoomBookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-dashboard"), AllowAnonymous]
    public async Task<ActionResult<ReservationDashboardDto>> GetDashboard()
    {
        var result = await _mediator.Send(new GetReservationDashboardQuery());

        return Ok(result);
    }

    [HttpGet("get-all-reservations"), AllowAnonymous]
    public async Task<ActionResult<List<ReservationWebDto>>> GetAllReservations()
    {
        var result = await _mediator.Send(new GetAllReservationsuery());

        return Ok(result);
    }
}

using System.Security.Claims;
using System.Threading;
using DHAFacilitationAPIs.Application.Feature.ConfirmBooking.Commands;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.AddRefundPolicy;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.CreateRefundPolicy.Dto;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.CreateRefundRequest;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.UpdateRefundPolicy;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.UpdateRefundRequest;
using DHAFacilitationAPIs.Application.Feature.Refund.Queries.GetRefundPolicy;
using DHAFacilitationAPIs.Application.Feature.Refund.Queries.GetRefundRequest_web_;
using DHAFacilitationAPIs.Application.Feature.Refunds.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.DeleteRoomCharges;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.AllReservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationDashboard;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

    [HttpGet("get-dashboard")]
    public async Task<ActionResult<ReservationDashboardDto>> GetDashboard()
    {
        var result = await _mediator.Send(new GetReservationDashboardQuery());

        return Ok(result);
    }

    [HttpGet("get-all-reservations")]
    public async Task<ActionResult<List<ReservationWebDto>>> GetAllReservations()
    {
        var result = await _mediator.Send(new GetAllReservationsuery());

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

    [HttpPost("create-RefundPolicy"), AllowAnonymous]
    public async Task<ActionResult<Guid>> CreateRefundPolicy([FromBody] CreateRefundPolicyDto dto)
    {
        var id = await _mediator.Send(new CreateRefundPolicyCommand(dto));
        return Ok(id);
    }

    [HttpPut("update-RefundPolicy"), AllowAnonymous]
    public async Task<ActionResult<Guid>> UpdateRefundPolicy([FromBody] UpdateRefundPolicyCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpDelete("delete-RefundPolicy"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRefundPolicyCommand([FromQuery] DeleteRefundPolicyCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("get-RefundPolicy/{clubId}"), AllowAnonymous]
    public async Task<IActionResult> GetRefundPoliciesByClub(Guid clubId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRefundPolicyQuery(clubId), cancellationToken);
        return Ok(result.Data);
    }

    [HttpPut("update-RefundRequest"), AllowAnonymous]
    public async Task<IActionResult> UpdateRefundRequest([FromBody] UpdateRefundRequestCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpGet("get-RefundRequest/{clubId:guid}"), AllowAnonymous]
    public async Task<ActionResult<List<RefundRequestWebDto>>> GetRefundRequestsByClub(Guid clubId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRefundRequestsWebQuery(clubId), ct);
        return Ok(result);
    }

}

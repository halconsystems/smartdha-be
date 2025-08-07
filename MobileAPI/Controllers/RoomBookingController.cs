using System.Security.Claims;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controller;
[ApiController]
[Route("api/[controller]")]
public class RoomBookingController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomBookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-user-clubs")]
    public async Task<IActionResult> GetUserClubs()
    {
        //return Ok(await _mediator.Send(new GetUserClubsQuery()));
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _mediator.Send(new GetUserClubsQuery { UserId = userId });

        return Ok(result);
    }

}

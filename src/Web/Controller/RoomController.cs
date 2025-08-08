using DHAFacilitationAPIs.Application.Feature.Room.Commands.CreateRoom;
using DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.CreateRoomCategory;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IMediator _mediator;
    public RoomController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateRoomCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));
}

using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class RoomAvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;
    public RoomAvailabilityController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    //[HttpPost("Create"), AllowAnonymous]
    //public async Task<ActionResult<SuccessResponse<Guid>>> Create([FromBody] CreateRoomAvailabilityCommand command, CancellationToken ct)
    //{
    //    var result = await _mediator.Send(command, ct);
    //    return Ok(result);
    //}
}


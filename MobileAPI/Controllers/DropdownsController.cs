using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[ApiController]
[Route("api/dropdowns"),AllowAnonymous]
public class DropdownsController : BaseApiController
{
    private readonly IMediator _mediator;

    public DropdownsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("clubs")]
    public async Task<ActionResult<List<DropdownDto>>> Clubs(CancellationToken ct)
        => Ok(await _mediator.Send(new GetDropdownQuery<Club>(), ct));


    [HttpGet("residence-types"), AllowAnonymous]
    public async Task<ActionResult<List<DropdownDto>>> GetResidenceTypes(CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<ResidenceType>(), ct);
        return Ok(data);
    }

    [HttpGet("room-categories"), AllowAnonymous]
    public async Task<ActionResult<List<DropdownDto>>> GetRoomCategories(CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<RoomCategory>(), ct);
        return Ok(data);
    }

    [HttpGet("services"), AllowAnonymous]
    public async Task<ActionResult<List<DropdownDto>>> GetServices(CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<DHAFacilitationAPIs.Domain.Entities.Services>(), ct);
        return Ok(data);
    }
}

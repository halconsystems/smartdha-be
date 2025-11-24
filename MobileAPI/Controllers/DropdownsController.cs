using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;

[ApiController]
[Route("api/dropdowns")]
public class DropdownsController : BaseApiController
{
    private readonly IMediator _mediator;

    public DropdownsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("clubs")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<List<DropdownDto>>> Clubs(CancellationToken ct)
        => Ok(await _mediator.Send(new GetDropdownQuery<Club>(), ct));


    [HttpGet("residence-types")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<List<DropdownDto>>> GetResidenceTypes(CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<ResidenceType>(), ct);
        return Ok(data);
    }

    [HttpGet("room-categories")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<List<DropdownDto>>> GetRoomCategories(CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<RoomCategory>(), ct);
        return Ok(data);
    }

    [HttpGet("services")]
    [ModuleAuthorize(Modules.Club)]
    public async Task<ActionResult<List<DropdownDto>>> GetServices(CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<DHAFacilitationAPIs.Domain.Entities.Services>(), ct);
        return Ok(data);
    }
}

using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class ClubsController : BaseApiController
{
    private readonly IMediator _mediator;
    public ClubsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Create"),AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, bool hardDelete, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteClubCommand(id, hardDelete), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Club?>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetClubByIdQuery(id), ct));

    [HttpGet,AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<Club>>> GetAll([FromQuery] bool includeInactive, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetClubsQuery(includeInactive, page, pageSize), ct));
}


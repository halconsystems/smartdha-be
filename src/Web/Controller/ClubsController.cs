using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubBookingStandardTimes;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
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

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteClubCommand(id), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Club?>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetClubByIdQuery(id), ct));

    [HttpGet]
    public async Task<ActionResult<List<ClubDto>>> GetAll([FromQuery] ClubType clubType)
        => Ok(await _mediator.Send(new GetClubsQuery(clubType)));

    [HttpPost("Create-Club Booking Standard Time"), AllowAnonymous]
    public async Task<IActionResult> CreateClubBookingStandardTime([FromBody] ClubBookingStandardTimeDto dto, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateClubBookingStandardTimeCommand { Dto = dto }, ct);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpPut("Update-Club Booking Standard Time"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateClubBookingStandardTime([FromBody] UpdateClubBookingStandardTimeCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Club Booking Standard Time"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeleteClubBookingStandardTime(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteClubBookingStandardTimeCommand { Id = id }, ct);
        return StatusCode(result.Status, result);
    }

    [HttpGet("GetAll-Club Booking Standard Time"), AllowAnonymous]
    public async Task<IActionResult> GetClubBookingStandardTimes(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClubBookingStandardTimesQuery(), cancellationToken);
        return Ok(result);
    }
}

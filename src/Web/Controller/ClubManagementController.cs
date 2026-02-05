using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubBookingStandardTimes;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "club")]
public class ClubManagementController : BaseApiController
{
    private readonly IMediator _mediator;
    public ClubManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Create-Club Booking Standard Time"), AllowAnonymous]
    public async Task<IActionResult> CreateClubBookingStandardTime([FromBody] ClubBookingStandardTimeDto dto, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateClubBookingStandardTimeCommand { Dto = dto }, ct);
        return CreatedAtAction(nameof(Application.Feature.Room_Availability.Command.Create), new { id }, id);
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

    [HttpGet("Get-Club Booking Standard Time"), AllowAnonymous]
    public async Task<IActionResult> GetClubBookingStandardTimes([FromQuery] Guid? clubId, ClubType clubType, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClubBookingStandardTimesQuery { ClubId = clubId, ClubType = clubType }, cancellationToken);
        return Ok(result);
    }

    

    //[HttpGet]
    //public async Task<ActionResult<List<ClubDto>>> GetAll([FromQuery] ClubType clubType)
    //    => Ok(await _mediator.Send(new GetAllClubByTypeQuery(clubType)));

}

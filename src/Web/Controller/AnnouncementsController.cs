using DHAFacilitationAPIs.Application.Feature.Announcement.Queries.GetAllAnnouncements;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.DeleteAnnouncement;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.UpdateAnnouncement;
using DHAFacilitationAPIs.Application.Feature.Announcements.Queries.GetAllAnnouncementsById;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "property")]
public class AnnouncementsController : BaseApiController
{
    private readonly IMediator _mediator;
    public AnnouncementsController(IMediator mediator) => _mediator = mediator;



    [HttpPost("Create")]
    public async Task<IActionResult> CreateAnnouncements(AddAnnouncementCommand addAnnouncementCommand)
    {
        var result = await Mediator.Send(addAnnouncementCommand);
        return Ok(result);

    }

    [HttpGet("GetAll-Annoucements")]
    public async Task<IActionResult> GetAllAnnoucements()
    {
        return Ok(await Mediator.Send(new GetAllAnnouncementsQuery()));
    }

    [HttpGet("GetAnnouncementById/{id:guid}")]
    public async Task<IActionResult> GetAnnouncementById(Guid id, CancellationToken ct)
    {
        var query = new GetAnnouncementByIdQuery { Id = id };
        var result = await Mediator.Send(query, ct);
        return Ok(result);
    }



    /// <summary>Update an announcement</summary>
    [HttpPut("{id:guid}")]
    //[ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<SuccessResponse<string>>> Update(
    Guid id,
    [FromBody] UpdateAnnouncementCommand command,
    CancellationToken ct)
        {
            // trust route id; overwrite command.Id if needed
            command.Id = id;
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

    /// <summary>Delete an announcement (soft by default; hard via query)</summary>
    [HttpDelete("{id:guid}")]
    //[ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(
    Guid id,
    [FromQuery] bool hardDelete = false,
    CancellationToken ct = default)
        {
            var result = await _mediator.Send(new DeleteAnnouncementCommand
            {
                Id = id,
                HardDelete = hardDelete
            }, ct);

            return Ok(result);
        }
    
}

using DHAFacilitationAPIs.Application.Feature.Announcement.Queries.GetAllAnnouncements;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class AnnouncementsController : BaseApiController
{
    [HttpPost("Create"), AllowAnonymous]
    public async Task<IActionResult> CreateAnnouncements(AddAnnouncementCommand addAnnouncementCommand)
    {
        var result = await Mediator.Send(addAnnouncementCommand);
        return Ok(result);

    }

    [HttpGet("GetAll-Annoucements"),AllowAnonymous]
    public async Task<IActionResult> GetAllAnnoucements()
    {
        return Ok(await Mediator.Send(new GetAllAnnouncementsQuery()));
    }

}

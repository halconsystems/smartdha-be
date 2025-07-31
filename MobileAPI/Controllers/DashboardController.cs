using DHAFacilitationAPIs.Application.Feature.Announcement.Queries.GetAllAnnouncements;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Commands.AddMemberTypeModuleAssignments;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.GetAllModules;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetCurrentUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : BaseApiController
{
    [HttpGet("GetAvailable-Modules")]
    public async Task<IActionResult> GetAllModules()
    {
        return Ok(await Mediator.Send(new GetAllModulesQuery()));
    }

    [HttpGet("GetAll-Annoucements")]
    public async Task<IActionResult> GetAllAnnoucements()
    {
        return Ok(await Mediator.Send(new GetAllAnnouncementsQuery()));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        return Ok(await Mediator.Send(new GetCurrentUserProfileQuery()));
    }

    
}

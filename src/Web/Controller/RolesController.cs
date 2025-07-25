using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAllRoles;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAssignableRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class RolesController : BaseApiController
{
    private readonly IUser _loggedInUser;
    public RolesController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllRoles()
    {
        return Ok(await Mediator.Send(new GetAllRolesQuery()));
    }
    
    [HttpGet("get-assignable-roles")]
    public async Task<IActionResult> GetAssignableRoles()
    {
        var currentUserId = _loggedInUser.Id;
        if (string.IsNullOrWhiteSpace(currentUserId))
            return Unauthorized("Unable to determine current user.");

        var result = await Mediator.Send(new GetAssignableRolesQuery(currentUserId));
        return Ok(result);
    }
}

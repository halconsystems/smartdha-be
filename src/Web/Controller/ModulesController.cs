using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetUserModulePermissions;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAssignableRoles;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
public class ModulesController : BaseApiController
{
    private readonly IUser _loggedInUser;
    public ModulesController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }

    [HttpGet("get-userpermissions/{userId}")]
    public async Task<IActionResult> GetUserModulePermissions(string userId)
    {
        var result = await Mediator.Send(new GetUserModulePermissionsQuery(userId));
        return Ok(result);
    }

    [HttpGet("get-assignable-modules")]
    public async Task<IActionResult> GetAssignableModules()
    {
        var currentUserId = _loggedInUser.Id;

        if (string.IsNullOrWhiteSpace(currentUserId))
            return Unauthorized("Unable to determine current user.");

        var result = await Mediator.Send(new GetAssignableModulesQuery(currentUserId));
        return Ok(result);
    }

}

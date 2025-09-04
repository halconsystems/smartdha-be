using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Commands.AddMemberTypeModuleAssignments;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.AddModule;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.DeleteModule;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateModule;
using DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetModule;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAssignableRoles;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdministrator")]
public class ModulesController : BaseApiController
{
    private readonly IUser _loggedInUser;
    public ModulesController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }


    [HttpPost("add-module")]
    public async Task<IActionResult> AddModule(AddModuleCommand addModuleCommand)
    {
        return Ok(await Mediator.Send(addModuleCommand));
    }

    [HttpGet("get-modules"), AllowAnonymous]
    public async Task<IActionResult> GetModules([FromQuery] string? id)
    {
        var result = await Mediator.Send(new GetModulesQuery { Id = id });
        return Ok(result);
    }

    [HttpPut("update-module")]
    public async Task<IActionResult> UpdateModule(UpdateModuleCommand updateModuleCommand)
    {
        return Ok(await Mediator.Send(updateModuleCommand));
    }

    [HttpDelete("delete-module")]
    public async Task<IActionResult> DeleteModule(DeleteModuleCommand deleteModuleCommand)
    {
        return Ok(await Mediator.Send(deleteModuleCommand));
       
    }

}

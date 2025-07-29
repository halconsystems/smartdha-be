using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.AddModule;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateModule;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.AddSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.DeleteSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.UpdateSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class SubModuleController : BaseApiController
{
    private readonly IUser _loggedInUser;
    public SubModuleController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }


    [HttpPost("add-submodule")]
    public async Task<IActionResult> AddSubModule(AddSubModuleCommand addSubModuleCommand)
    {
        return Ok(await Mediator.Send(addSubModuleCommand));
    }

    [HttpPost("update-submodule")]
    public async Task<IActionResult> UpdateSubModule(UpdateSubModuleCommand updateSubModuleCommand)
    {
        return Ok(await Mediator.Send(updateSubModuleCommand));
    }

    [HttpPost("delete-submodule")]
    public async Task<IActionResult> deleteSubModule(DeleteSubModuleCommand deleteSubModuleCommand)
    {
        var result=await Mediator.Send(deleteSubModuleCommand);
        return result
    ? Ok(new { message = "Sub-Module deleted (soft delete) successfully." })
    : BadRequest(new { message = "Failed to delete Sub-Module." });
    }

    [HttpGet("get-submodules"), AllowAnonymous]
    public async Task<IActionResult> GetSubModuleList([FromQuery] string? id)
    {
        Guid? submoduleId = null;
        if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out var parsedId))
        {
            submoduleId = parsedId;
        }
        var result = await Mediator.Send(new SubModuleListQuery { Id = submoduleId });
        return Ok(result);
    }



}

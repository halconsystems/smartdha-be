using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetModule;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.AddSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.DeleteSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.UpdateSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserById;
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

    [HttpGet("get-submodule"), AllowAnonymous]
    public async Task<IActionResult> GetModules([FromQuery] string? id)
    {
        if (!Guid.TryParse(id, out var userGuid))
            return BadRequest("Invalid user id format.");

        var result = await Mediator.Send(new GetSubModuleListQuery { Id = userGuid });
        return Ok(result);
    }



    [HttpPut("update-submodule")]
    public async Task<IActionResult> UpdateSubModule(UpdateSubModuleCommand updateSubModuleCommand)
    {
        return Ok(await Mediator.Send(updateSubModuleCommand));
    }

    [HttpDelete("delete-submodule")]
    public async Task<IActionResult> deleteSubModule(DeleteSubModuleCommand deleteSubModuleCommand)
    {
        return Ok(await Mediator.Send(deleteSubModuleCommand));

    }
}

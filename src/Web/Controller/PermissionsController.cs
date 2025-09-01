using DHAFacilitationAPIs.Application.Feature.Permissions.Commands.CreatePermission;
using DHAFacilitationAPIs.Application.Feature.SubModules.Commands.AddSubModule;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class PermissionsController : BaseApiController
{

    [HttpPost("add-permission")]
    public async Task<IActionResult> CreatePermission([FromForm] CreatePermissionCommand cmd, CancellationToken ct)
    {
        return Ok(await Mediator.Send(cmd));
    }

    [HttpGet("get-submodules-permissions")]
    public async Task<IActionResult> GetSubModulePermissionList([FromQuery] string? id)
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

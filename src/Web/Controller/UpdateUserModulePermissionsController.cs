using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.User.Commands;
using DHAFacilitationAPIs.Application.Feature.User.Queries;
using DHAFacilitationAPIs.Web.Controller;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class UserManagementController : BaseApiController
{
    private readonly IUser _loggedInUser;



    public UserManagementController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }

    //[HttpPost("Update-User-Permissions"), AllowAnonymous]
    //public async Task<IActionResult> UpdateUserModulePermissions([FromBody] UpdateUserModulePermissionsCommand request)
    //{
    //    var result = await Mediator.Send(request);
    //    return Ok(new { message = result });
    //}
}

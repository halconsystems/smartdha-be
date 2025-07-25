using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAllRoles;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ActivateDeactivateUser;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseApiController
{
    private readonly IUser _loggedInUser;
    public UsersController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }

    [HttpPost("Login"), AllowAnonymous]
    public async Task<IActionResult> GetToken(GenerateTokenCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(RegisterUserCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
    
    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await Mediator.Send(new GetAllUsersQuery()));
    }

    [HttpPost("update-activation")]
    public async Task<IActionResult> UpdateActivation([FromBody] ActivateDeactivateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return result
            ? Ok(new { message = $"User has been {(command.IsActive ? "activated" : "deactivated")} successfully." })
            : BadRequest(new { message = "Failed to update user status." });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand command)
    {
        var result = await Mediator.Send(command);
        return result
            ? Ok(new { message = "User deleted (soft delete) successfully." })
            : BadRequest(new { message = "Failed to delete user." });
    }


   

}

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAllRoles;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ActivateDeactivateUser;
using DHAFacilitationAPIs.Application.Feature.User.Commands.AdminResetPassword;
using DHAFacilitationAPIs.Application.Feature.User.Commands.CreateRole;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ResetPassword;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UpdatePassword;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateUser;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllUsers;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetRoles;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserDashboard;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserWithAccess;
using DHAFacilitationAPIs.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Auth")]
public class UsersController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    public UsersController(IUser loggedInUser, IMediator mediator)
    {
        _loggedInUser = loggedInUser;
        _mediator = mediator;
    }

    [HttpPost("Login"), AllowAnonymous]
    public async Task<IActionResult> GetToken(GenerateTokenCommand request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpPost("Create-Role")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command, CancellationToken ct)
    {
        var role = await _mediator.Send(command, ct);
        return Ok(role);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(RegisterUserCommand request)
    {
        return Ok(await _mediator.Send(request));
    }
    
    [HttpPut("update-activation")]
    public async Task<IActionResult> UpdateActivation([FromBody] ActivateDeactivateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return result
            ? Ok(new { message = $"User has been {(command.IsActive ? "activated" : "deactivated")} successfully." })
            : BadRequest(new { message = "Failed to update user status." });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand command)
    {
        var result = await Mediator.Send(command);
        return result
            ? Ok(new { message = "User deleted (soft delete) successfully." })
            : BadRequest(new { message = "Failed to delete user." });
    }

    [HttpPut("Update-User")]
    public async Task<IActionResult> UpdateUserAccess([FromBody] UpdateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("user-dashboard")]
    public async Task<ActionResult<SuccessResponse<UserDashboardDto>>> GetUserDashboard()
    {
        var result = await _mediator.Send(new GetUserDashboardQuery());
        return Ok(result);
    }

    
    [HttpGet("GetRoles"), AllowAnonymous]
    public async Task<IActionResult> GetRoles()
    {
        var result = await Mediator.Send(new GetRolesQuery());
        return Ok(result);
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await Mediator.Send(new GetAllUsersQuery()));
    }

    [HttpGet("Get-AccessTree")]
    public async Task<IActionResult> GetAccessTree()
    {
        return Ok(await Mediator.Send(new GetAccessTreeQuery()));
    }

    [HttpGet("GetUserAccessById")]
    public async Task<IActionResult> GetUserAccessById([FromQuery] string userId)
    {
        var result = await Mediator.Send(new GetUserWithAccessQuery(userId));
        return Ok(result);
    }


    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordCommand command, CancellationToken ct)
    => Ok(await _mediator.Send(command, ct));

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("admin-reset-password")]
    [Authorize(Roles = "SuperAdministrator")]
    public async Task<IActionResult> AdminResetPassword(AdminResetPasswordCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));



    //[HttpGet("GetUserById"), AllowAnonymous]
    //public async Task<IActionResult> GetUserById([FromQuery]string id)
    //{
    //    var result = await Mediator.Send(new GetUserByIdQuery { UserId = id });
    //    return result is null ? NotFound() : Ok(result);
    //}






}

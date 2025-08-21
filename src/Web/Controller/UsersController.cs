using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAllRoles;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ActivateDeactivateUser;
using DHAFacilitationAPIs.Application.Feature.User.Commands.CreateRole;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllUsers;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetRoles;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
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


    [HttpGet("GetRoles")]
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

    [HttpGet("GetUserById"), AllowAnonymous]
    public async Task<IActionResult> GetUserById([FromQuery]string id)
    {
        var result = await Mediator.Send(new GetUserByIdQuery { UserId = id });
        return result is null ? NotFound() : Ok(result);
    }




}

using System.Data;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.UpdateUserFamilyCommandHandler;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;
using DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.DeleteUserFamilyCommand;
using DHAFacilitationAPIs.Application.Feature.Worker.Commands.DeleteWorker;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Infrastructure.Data.SQLite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class UserFamilyController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    public UserFamilyController(IUser loggedInUser, IMediator mediator)
    {
        _loggedInUser = loggedInUser;
        _mediator = mediator;
    }

    [HttpPost("add-user-family"), /*Authorize(Roles = AllRoles.Member)*/ AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddUserFamily([FromForm] AddUserFamilyCommand request)
    {
        if (!string.IsNullOrWhiteSpace(_loggedInUser?.Id) && Guid.TryParse(_loggedInUser.Id, out var uid))
        {
            request.UserId = uid;
        }

        var result = await _mediator.Send(request);
        return Ok(ApiResult<Guid>.Ok(result.Data, "Family member added successfully"));
    }

    [HttpPost("update-user-family"), /*Authorize(Roles = AllRoles.Member)*/ AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUserFamily([FromForm] UpdateUserFamilyCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResult<UpdateUserFamilyResponse>.Ok(result.Data!, "Family member updated successfully"));
    }
    [HttpPost("get-all-users")]
    //[Authorize(Roles = AllRoles.Member)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(GetAllUserFamilyQuery request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResult<List<GetAllUserFamilyQueryResponse>>.Ok(result.Data!.ToList(), "Record fetched successfully"));
    }

    [HttpGet("get-user-by-id/{id:guid}")]
    //[Authorize(Roles = AllRoles.Member)]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserFamilyByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        if (!result.Succeeded)
            return BadRequest(ApiResult<string>.Fail(result.Errors.First()));

        return Ok(ApiResult<GetUserFamilybyIdQueryResponse>.Ok(result.Data!, "Record fetched successfully"));
    }
    [HttpPost("delete-user-family"), AllowAnonymous]
    public async Task<IActionResult> DeleteUserFamily([FromBody] DeleteUserFamilyCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result.Succeeded)
            return BadRequest(ApiResult<DeleteUserFamilyCommand>.Fail(result.Errors.First()));
        return Ok(ApiResult<Guid>.Ok(result.Data, "user family deleted successfully"));
    }
}

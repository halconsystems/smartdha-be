using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.UpdateUserFamilyCommandHandler;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("add-user-family"), AllowAnonymous]
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

    [HttpPost("update-user-family"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUserFamily([FromForm] UpdateUserFamilyCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResult<UpdateUserFamilyResponse>.Ok(result.Data!, "Family member updated successfully"));
    }
    [HttpGet("get-all-users")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUserFamilyQuery());
        return Ok(ApiResult<List<GetAllUserFamilyQueryResponse>>.Ok(result.Data!.ToList(), "Record fetched successfully"));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserFamilyByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        if (!result.Succeeded)
            return BadRequest(ApiResult<string>.Fail(result.Errors.First()));

        return Ok(ApiResult<GetUserFamilybyIdQueryResponse>.Ok(result.Data!, "Record fetched successfully"));
    }
}

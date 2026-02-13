using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
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

    //[HttpPost("add-user-family"), AllowAnonymous]
    //public async Task<IActionResult> AddUserFamily([FromBody]AddUserFamilyCommand request)
    //{
    //    return Ok(await _mediator.Send(request));
    //}

    [HttpPost("add-user-family"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddUserFamily([FromForm] AddUserFamilyCommand request)
    {
        // optional: set UserId from currently logged in user (if available and you want it populated)
        if (!string.IsNullOrWhiteSpace(_loggedInUser?.Id) && Guid.TryParse(_loggedInUser.Id, out var uid))
        {
            request.UserId = uid;
        }

        var result = await _mediator.Send(request);
        return Ok(result);
    }
}

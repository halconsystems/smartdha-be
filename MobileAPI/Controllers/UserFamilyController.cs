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

    [HttpPost("add-user-family"), AllowAnonymous]
    public async Task<IActionResult> GetToken(AddUserFamilyCommand request)
    {
        return Ok(await _mediator.Send(request));
    }
}

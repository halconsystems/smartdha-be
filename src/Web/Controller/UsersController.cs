using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
public class UsersController : BaseApiController
{
    [HttpPost("GetToken"), AllowAnonymous]
    public async Task<IActionResult> GetToken(GenerateTokenCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
    [HttpPost("Register"),AllowAnonymous]
    public async Task<IActionResult> RegisterUser(RegisterUserCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}

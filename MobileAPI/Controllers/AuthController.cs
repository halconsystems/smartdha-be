using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
using DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterNonMember;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ResendOtp;
using DHAFacilitationAPIs.Application.Feature.User.Commands.SetPassword;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.Feature.User.Queries.VerifyOtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseApiController
{
    [HttpPost("Login"), AllowAnonymous]
    public async Task<IActionResult> AppLogin(AppUserLoginCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("Register"), AllowAnonymous]
    public async Task<IActionResult> RegisterUser(MemberRegisterationCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("Verify-OTP"), AllowAnonymous]
    public async Task<IActionResult> VerifyOTP(VerifyOtpCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("Resend-OTP"), AllowAnonymous]
    public async Task<IActionResult> ResendOTP(ResendOtpCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [Authorize(Policy = "SetPasswordPolicy")]
    [HttpPost("Set-Password")]
    public async Task<IActionResult> SetPassword(SetPasswordCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("NonMember-Purpose"), AllowAnonymous]
    public async Task<IActionResult> NonMemberPurpose()
    {
        return Ok(await Mediator.Send(new GetAllNonMemberPurposesQuery()));
    }

    [HttpPost("Register-NonMember"), AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromForm] RegisterNonMemberCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}

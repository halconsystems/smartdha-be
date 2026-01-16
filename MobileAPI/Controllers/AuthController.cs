using System.Security.Claims;
using DHAFacilitationAPIs.Application.Feature.ForgetPassword.Command;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
using DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RefreshToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterNonMember;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ResendOtp;
using DHAFacilitationAPIs.Application.Feature.User.Commands.SetPassword;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.Feature.User.Queries.VerifyOtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "auth")]
public class AuthController : BaseApiController
{
    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("Login")]
    public async Task<IActionResult> AppLogin(AppUserLoginCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [Authorize(Policy = "SetOTPPolicy")]
    [HttpPost("Verify-OTP")]
    public async Task<IActionResult> VerifyOTP(VerifyOtpCommand request)
    {
        var purpose = User.FindFirstValue("purpose");

        if (purpose != "verify_otp")
            return Forbid(); // extra defense in depth

        return Ok(await Mediator.Send(request));
    }

    [Authorize(Policy = "SetOTPPolicy")]
    [HttpPost("Resend-OTP")]
    public async Task<IActionResult> ResendOTP(ResendOtpCommand request)
    {
         var purpose = User.FindFirstValue("purpose");

        if (purpose != "verify_otp")
            return Forbid(); // extra defense in depth
        return Ok(await Mediator.Send(request));
    }

   
    [Authorize(Policy = "SetPasswordPolicy")]
    [HttpPost("Set-Password")]
    public async Task<IActionResult> SetPassword(SetPasswordCommand request)
    {
        var purpose = User.FindFirstValue("purpose");

        if (purpose != "set_password")
            return Forbid(); // extra defense in depth
        return Ok(await Mediator.Send(request));
    }


    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(MemberRegisterationCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("NonMember-Purpose")]
    public async Task<IActionResult> NonMemberPurpose()
    {
        return Ok(await Mediator.Send(new GetAllNonMemberPurposesQuery()));
    }

    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("Register-NonMember")]
    public async Task<IActionResult> RegisterUser([FromForm] RegisterNonMemberCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    [AllowAnonymous]
    [HttpPost("Forget-Password")]
    public async Task<IActionResult> forgetPassword(ForgetPasswordCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
    [AllowAnonymous]
    [HttpPost("Send-OTP")]
    public async Task<IActionResult> SendOTP(OTPSendCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}

using System.Security.Claims;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.ForgetPassword.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Queries;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.PMS.Commands.AddCaseAttachment;
using DHAFacilitationAPIs.Application.Feature.Profile.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.LiveMemberRegisteration;
using DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
using DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RefreshToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterNonMember;
using DHAFacilitationAPIs.Application.Feature.User.Commands.ResendOtp;
using DHAFacilitationAPIs.Application.Feature.User.Commands.SetPassword;
using DHAFacilitationAPIs.Application.Feature.User.Commands.SubmitUserDeleteRequest;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UserImage;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.Feature.User.Queries.VerifyOtp;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using DHAFacilitationAPIs.Infrastructure.Data.SQLite;
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

    private readonly IFileStorageService _files;

    public AuthController(IFileStorageService files)
    {
        _files = files;
    }
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
   // public async Task<IActionResult> RegisterUser(LiveMemberRegisterationCommand request)
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
    //[Authorize(Policy = "SetPasswordPolicy")]
    //[HttpPost("Forget-Password")]
    //public async Task<IActionResult> forgetPassword(ForgetPasswordCommand request)
    //{
    //    var purpose = User.FindFirstValue("purpose");

    //    if (purpose != "set_password")
    //        return Forbid(); // extra defense in depth
    //    return Ok(await Mediator.Send(request));
    //}
    
    //public async Task<IActionResult> forgetPassword(ForgetPasswordCommand request)
    //{
    //    return Ok(await Mediator.Send(request));
    //}
    [AllowAnonymous]
    [HttpPost("Reset-Password")]
    public async Task<IActionResult> ResetPassword(OTPSendCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [Authorize(Policy = "SetOTPPolicy")]
    [HttpPost("Verify-FrogetPassword-OTP")]
    public async Task<IActionResult> VerifyOTPFrogetPassword(VerifyOtpForPasswordResetCommand request)
    {
        var purpose = User.FindFirstValue("purpose");

        if (purpose != "verify_otp")
            return Forbid(); // extra defense in depth

        return Ok(await Mediator.Send(request));
    }

    [Authorize]
    [HttpPost("users/delete-request")]
    public async Task<IActionResult> SubmitDeleteRequest(
    [FromBody] SubmitUserDeleteRequestDto dto,
    CancellationToken ct)
    {
        ApiResult<Guid> result =
            await Mediator.Send(
                new SubmitUserDeleteRequestCommand(dto.Reason),
                ct);

        return Ok(result);
    }

    [HttpGet("GetProfileDetail")]
    public async Task<IActionResult> GetRoomDetails()
    {
        var result = await Mediator.Send(new GetProfileDetailsQuery());
        return Ok(result);
    }

    [HttpPost("User/Profileimages/add")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> AddCaseAttachments(
         [FromForm] AddUserImagesFlatForm form,
         CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (form.Files == null)
            return BadRequest("No images uploaded.");

        var folder = $"UserProfile/{form.ImageNames}";

        var file = form.Files;

        var filePath = await _files.SaveFileAsync(
                file,
                folder,
                ct,
                maxBytes: 10 * 1024 * 1024,
                allowedExtensions: new[] { ".jpg", ".jpeg", ".png", ".pdf" }
            );

        var ext = Path.GetExtension(filePath);

        // Pick metadata by index (or defaults)
        var name = form.ImageNames;
        var desc = form.Descriptions;
        var cat = form.Categories;

        var uploaded = new AddProfileImageDTO(
        ImageURL: filePath,
        ImageExtension: ext,
        ImageName: name,
        Description: desc,
        Category: ImageCategory.Main
        );


        // ✅ Send only file path to CQRS
        var cmd = new AddUserImagesCommand(
            uploaded
        );

        //lastResult = await Mediator.Send(cmd, ct);
        var result = await Mediator.Send(cmd, ct);
        return Ok(result);

    }
}
public class SubmitUserDeleteRequestDto
{
    public string? Reason { get; set; }
}

using Azure.Core;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class NonMemberController : BaseApiController
{
    private readonly IUser _loggedInUser;

    public NonMemberController(IUser loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }

    [HttpGet("get-nonmember-requests"), AllowAnonymous]
    public async Task<IActionResult> GetNonMemberRequests()
    {
        var result = await Mediator.Send(new NonMemberRequestsQuery());
        return Ok(result);
    }


    [HttpPost("Update-nonmember-requests"), AllowAnonymous]
    public async Task<IActionResult> UpdateNonMemberRequests(UpdateNonMemberVerificationCommand updateNonMemberVerificationCommand)
    {
        var result = await Mediator.Send(updateNonMemberVerificationCommand);
        return Ok(result);

    }

}

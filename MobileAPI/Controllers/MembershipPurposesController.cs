using Azure.Core;
using DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Commands.AddMembershipPurpose;
using DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Queries.GetAllMembershipPurposes;
using DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
using DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MembershipPurposesController : BaseApiController
{
    [HttpGet("GetMembershipPurposes"), AllowAnonymous]
    public async Task<IActionResult> GetMembershipPurposes()
    {
        return Ok(await Mediator.Send(new GetAllMembershipPurposesQuery()));
    }

    //[HttpPost("AddMembershipPurposes")]

    //public async Task<IActionResult> AddMembershipPurposes(AddMembershipPurposeCommand request)
    //{
    //    return Ok(await Mediator.Send(request));
    //}
}

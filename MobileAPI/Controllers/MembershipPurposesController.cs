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
    [HttpPost("GetMembershipPurposes"), AllowAnonymous]
    public async Task<IActionResult> GetMembershipPurposes(GetAllMembershipPurposesQuery request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("AddMembershipPurposes"), AllowAnonymous]
    public async Task<IActionResult> AddMembershipPurposes(AddMembershipPurposeCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}

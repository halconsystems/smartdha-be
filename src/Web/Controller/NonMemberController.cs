using Azure.Core;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Commands.AddMemberTypeModuleAssignments;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.NonMemberApproval;
using DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class NonMemberController : BaseApiController
{
    private readonly IMediator _mediator;

    public NonMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Dashboard"), AllowAnonymous]
    public Task<SuccessResponse<NonMemberVerificationDashboardDto>> Get(CancellationToken ct)
        => _mediator.Send(new GetNonMemberVerificationDashboardQuery(), ct);

    [HttpPost("get-nonmember-requests"), AllowAnonymous]
    public async Task<IActionResult> GetNonMemberRequests(NonMemberRequestsQuery nonMemberRequestsQuery)
    {
        var result = await Mediator.Send(nonMemberRequestsQuery);
        return Ok(result);
    }


    [HttpPost("Update-nonmember-requests"), AllowAnonymous]
    public async Task<IActionResult> UpdateNonMemberRequests(UpdateNonMemberVerificationCommand updateNonMemberVerificationCommand)
    {
        var result = await Mediator.Send(updateNonMemberVerificationCommand);
        return Ok(result);

    }

    

}

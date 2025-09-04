using Azure.Core;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Commands.AddMemberTypeModuleAssignments;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.NonMemberApproval;
using DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetMobileModules;
using DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
using DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.Assignment;
using DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.DeleteAssignment;
using DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.InactiveAssignment;
using DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Queries.GetAssignedModules;
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

    [HttpGet("Dashboard")]
    public Task<SuccessResponse<NonMemberVerificationDashboardDto>> Get(CancellationToken ct)
        => _mediator.Send(new GetNonMemberVerificationDashboardQuery(), ct);

    [HttpPost("get-nonmember-requests")]
    public async Task<IActionResult> GetNonMemberRequests(NonMemberRequestsQuery nonMemberRequestsQuery)
    {
        var result = await Mediator.Send(nonMemberRequestsQuery);
        return Ok(result);
    }


    [HttpPost("Update-nonmember-requests")]
    public async Task<IActionResult> UpdateNonMemberRequests(UpdateNonMemberVerificationCommand updateNonMemberVerificationCommand)
    {
        var result = await Mediator.Send(updateNonMemberVerificationCommand);
        return Ok(result);

    }
    [HttpPost("AssignModule")]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AssignModule([FromBody] CreateUserModuleAssignmentCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Inactivate")]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> Inactivate([FromBody] InactivateUserModuleAssignmentCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("Delete")]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> Delete([FromQuery] DeleteUserModuleAssignmentCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("GetAssignedModules/{userId}")]
    public async Task<ActionResult<SuccessResponse<List<AssignedModuleDto>>>> GetAssignedModules(string userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAssignedModulesQuery { UserId = userId }, ct);
        return Ok(result);
    }

    [HttpGet("GetMobileModule")]
    public async Task<IActionResult> GetMobileModules()
    {
        var result = await _mediator.Send(new GetMobileModulesQuery());
        return Ok(result);
    }
}






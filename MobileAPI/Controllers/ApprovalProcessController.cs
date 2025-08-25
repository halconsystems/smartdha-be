using DHAFacilitationAPIs.Application.Feature.ApprovalProcess.Queries.GetProcessStepsByProcessId;
using DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Commands;
using DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetRequestTrackings;
using DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetTrackingStepsByTrackingId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ApprovalProcessController : BaseApiController
{
    [HttpGet("ProcessSteps/{ProcessId}")]
    public async Task<IActionResult> GetUserModulePermissions(int ProcessId)
    {
        var result = await Mediator.Send(new GetProcessStepsByProcessIdQuery(ProcessId));
        return Ok(result);
    }

    [HttpPost("SubmitRequest")]
    public async Task<IActionResult> SubmitRequest(CreateApprovalRequestCommand requestCommand)
    {
        return Ok(await Mediator.Send(requestCommand));

    }

    [HttpGet("AllRequest")]
    public async Task<IActionResult> AllRequest()
    {
        var result = await Mediator.Send(new GetRequestTrackingsQuery());
        return Ok(result);
    }

    [HttpGet("GetRequestApprovalStatus/{TrackingId}")]
    public async Task<IActionResult> GetRequestApprovalStatus(long TrackingId)
    {
        var result = await Mediator.Send(new GetTrackingStepsByTrackingIdQuery(TrackingId));
        return Ok(result);
    }
}

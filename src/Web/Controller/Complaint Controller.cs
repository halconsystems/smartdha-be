using DHAFacilitationAPIs.Application.Feature.Complaints.Web.Commands.UpdateComplaintStatus;
using DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.ComplaintDashboard;
using DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetAllComplaints;
using DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetComplaintCategories;
using DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetComplaintPriorities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
public class ComplaintController : BaseApiController
{
    private readonly IMediator _mediator;

    public ComplaintController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-dashboard-summary"), AllowAnonymous]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var result = await _mediator.Send(new GetComplaintDashboardSummaryQuery());
        return Ok(result);
    }

    [HttpGet("get-all-complaints"), AllowAnonymous]
    public async Task<IActionResult> GetAllComplaints([FromQuery] ComplaintStatus? status = null, [FromQuery] string? categoryCode = null, [FromQuery] string? priorityCode = null)
    {
        var result = await _mediator.Send(new GetAllComplaintsQuery(status, categoryCode, priorityCode));
        return Ok(result);
    }

    [HttpGet("get-complaint-categories"), AllowAnonymous]
    public async Task<IActionResult> GetAllComplaintCategories()
    {
        var result = await _mediator.Send(new GetAllComplaintCategoriesQuery());
        return Ok(result);
    }

    [HttpGet("get-complaint-priorities"), AllowAnonymous]
    public async Task<IActionResult> GetAllComplaintPriorities()
    {
        var result = await _mediator.Send(new GetAllComplaintPrioritiesQuery());
        return Ok(result);
    }

    [HttpPut("update-complaint-status"), AllowAnonymous]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateComplaintStatusCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(new { message = result });
    }
}

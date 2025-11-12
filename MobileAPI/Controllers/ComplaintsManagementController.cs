using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Commands.CreateComplaint;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetComplaintDropdowns;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaints;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaintStatusCount;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComplaintsManagementController : BaseApiController
{
    private readonly IMediator _med;
    public ComplaintsManagementController(IMediator med) => _med = med;

    [HttpGet("Complaint-Categories"),AllowAnonymous]
    public Task<ComplaintDropdownVm> Types() => _med.Send(new GetComplaintDropdownsQuery());

    [HttpPost]
    [Authorize]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> CreateComplaint([FromForm] CreateComplaintRequest request)
    {
        var complaintNo = await Mediator.Send(new CreateComplaintCommand(request));
        return Ok(new { success = true, message = "Complaint submitted successfully.", complaintNo });
    }

    [HttpGet("GetComplaints")]
    [Authorize]
    public async Task<IActionResult> GetMyComplaints()
    {
        var result = await Mediator.Send(new GetMyComplaintsQuery());
        return Ok(result);
    }

    [HttpGet("Status-Summary")]
    [Authorize]
    public async Task<IActionResult> GetMyComplaintStatusSummary()
    {
        var result = await Mediator.Send(new GetMyComplaintStatusCountQuery());
        return Ok(result);
    }

}

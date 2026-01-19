using System.Text.Json;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.AddCaseDocument;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.SubmitCase_V1;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.SaveCasePrerequisiteValue;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetMyCasesByCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class CaseDataController : BaseApiController
{
    private readonly IMediator _mediator;
    public CaseDataController(IMediator mediator) => _mediator = mediator;

    [HttpPost("submit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Submit(
    [FromForm] SubmitCaseRequest request,
    CancellationToken ct)
    {
        var prereqs = JsonSerializer.Deserialize<List<PrerequisiteValueInput>>(
    request.PrerequisiteValuesJson,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

        var cmd = new SubmitCaseCommand(
            request.UserPropertyId,
            request.ProcessId,
            request.ApplicantName,
            request.ApplicantCnic,
            request.ApplicantMobile,
            request.ApplicantRemarks,
            prereqs!,
            request.Files
        );

        return Ok(await _mediator.Send(cmd, ct));
    }


    [HttpPost("submit_v1")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Submit_V1(
        [FromForm] SubmitCaseRequest request,
        CancellationToken ct)
    {
        var prereqs = JsonSerializer.Deserialize<List<PrerequisiteValueInput>>(
            request.PrerequisiteValuesJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (prereqs == null)
            return BadRequest("Invalid prerequisiteValuesJson.");

        var cmd = new SubmitCase_V1Command(
            request.UserPropertyId,
            request.ProcessId,
            request.ApplicantName,
            request.ApplicantCnic,
            request.ApplicantMobile,
            request.ApplicantRemarks,
            prereqs,
            request.Files
        );

        return Ok(await _mediator.Send(cmd, ct));
    }




    [HttpGet("hitory/{categoryId:guid}")]
    public async Task<IActionResult> GetMyCasesByCategory(
        Guid categoryId,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new GetMyCasesByCategoryQuery(categoryId), ct));

    [HttpGet("hitory")]
    public async Task<IActionResult> GetWorkflow()
       => Ok(await _mediator.Send(new GetMyCasesQuery()));

    [HttpGet("{caseId:guid}/workflow")]
    public async Task<IActionResult> GetWorkflow(Guid caseId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCaseWorkflowHierarchyQuery(caseId), ct));
}

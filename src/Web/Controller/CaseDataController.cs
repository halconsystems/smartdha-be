using System.Text.Json;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.AddCaseDocument;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.SaveCasePrerequisiteValue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DHAFacilitationAPIs.Web.Controller.PropertyCasesController;

namespace DHAFacilitationAPIs.Web.Controller;
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
            request.PrerequisiteValuesJson);

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

    //[HttpPost]
    //public async Task<IActionResult> Create(CreateCaseCommand cmd, CancellationToken ct)
    //    => Ok(await _mediator.Send(cmd, ct));

    //[HttpPost("{caseId:guid}/submit")]
    //public async Task<IActionResult> Submit(Guid caseId, [FromBody] SubmitCaseCommandBody body, CancellationToken ct)
    //=> Ok(await _mediator.Send(new SubmitCaseCommand(caseId, body.ApplicantRemarks), ct));

    //[HttpPost("prerequisites")]
    //public async Task<IActionResult> SavePrereq(SaveCasePrerequisiteValueCommand cmd, CancellationToken ct)
    //    => Ok(await _mediator.Send(cmd, ct));

    //[HttpPost("documents")]
    //[Consumes("multipart/form-data")]
    //public async Task<IActionResult> UploadDocument(
    //   [FromForm] AddCaseDocumentCommand request,
    //   CancellationToken ct)
    //{

    //    return Ok(await _mediator.Send(request, ct));
    //}

    [HttpGet("hitory")]
    public async Task<IActionResult> GetWorkflow()
       => Ok(await _mediator.Send(new GetMyCasesHistoryQuery()));

    [HttpGet("{caseId:guid}/workflow")]
    public async Task<IActionResult> GetWorkflow(Guid caseId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCaseWorkflowHierarchyQuery(caseId), ct));

    
}

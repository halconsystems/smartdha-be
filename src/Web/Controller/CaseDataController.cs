using System.Text.Json;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.AddCaseDocument;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetAdminCaseDetail;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetAllCasesForAdmin;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyCasesHistory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyModuleUsers;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeDefinitionByProcessId;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseResultDocument.Commands.UploadCaseResultDocument;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.SaveCasePrerequisiteValue;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.ForwardExternal;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.ForwardInternal;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.RejectCase;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.ReturnCase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class CaseDataController : BaseApiController
{
    private readonly IMediator _mediator;
    public CaseDataController(IMediator mediator) => _mediator = mediator;

    //[HttpPost("submit")]
    //[Consumes("multipart/form-data")]
    //public async Task<IActionResult> Submit(
    //[FromForm] SubmitCaseRequest request,
    //CancellationToken ct)
    //{
    //    var prereqs = JsonSerializer.Deserialize<List<PrerequisiteValueInput>>(
    //        request.PrerequisiteValuesJson);

    //    var cmd = new SubmitCaseCommand(
    //        request.UserPropertyId,
    //        request.ProcessId,
    //        request.ApplicantName,
    //        request.ApplicantCnic,
    //        request.ApplicantMobile,
    //        request.ApplicantRemarks,
    //        prereqs!,
    //        request.Files
    //    );

    //    return Ok(await _mediator.Send(cmd, ct));
    //}

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

    //[HttpGet("hitory")]
    //public async Task<IActionResult> GetWorkflow()
    //   => Ok(await _mediator.Send(new GetMyCasesQuery()));

    [HttpGet("Cases/{moduleId:guid}")]
    public async Task<IActionResult> GetAllCases(Guid moduleId,CancellationToken ct)
       => Ok(await _mediator.Send(new GetAllCasesForAdminQuery(moduleId), ct));

    [HttpGet("{caseId:guid}")]
    public async Task<IActionResult> GetCaseDetail(
    Guid caseId,
    CancellationToken ct)
    {
        return Ok(await _mediator.Send(
            new GetAdminCaseDetailQuery(caseId), ct));
    }

    [HttpGet("by-process/{processId:guid}")]
    public async Task<IActionResult> GetByProcess(
        Guid processId,
        CancellationToken ct)
    {
        return Ok(await _mediator.Send(
            new GetFeeDefinitionByProcessIdQuery(processId), ct));
    }

    [HttpGet("{caseId:guid}/workflow")]
    public async Task<IActionResult> GetWorkflow(Guid caseId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCaseWorkflowHierarchyQuery(caseId), ct));

    [HttpGet("modules/{moduleId:guid}/users")]
    public Task<ApiResult<List<ModuleUserDropdownDto>>> GetModuleUsers(Guid moduleId)
        => _mediator.Send(new GetModuleUsersQuery(moduleId));

    [HttpPost("{caseId:guid}/forward-internal")]
    public Task<ApiResult<bool>> ForwardInternal(Guid caseId, [FromBody] ForwardInternalDto dto)
        => _mediator.Send(new ForwardInternalCommand(caseId, dto.ToUserId, dto.Remarks));

    [HttpPost("{caseId:guid}/forward-external")]
    public Task<ApiResult<bool>> ForwardExternal(Guid caseId, [FromBody] RemarksDto dto)
        => _mediator.Send(new ForwardExternalCommand(caseId, dto.Remarks));

    [HttpPost("{caseId:guid}/reject")]
    public Task<ApiResult<bool>> Reject(Guid caseId, [FromBody] RemarksDto dto)
        => _mediator.Send(new RejectCaseCommand(caseId, dto.Remarks!));

    [HttpPost("{caseId:guid}/return")]
    public Task<ApiResult<bool>> Return(Guid caseId, [FromBody] RemarksDto dto)
        => _mediator.Send(new ReturnCaseCommand(caseId, dto.Remarks!));

    [HttpPost("{caseId:guid}/result-document")]
    public async Task<IActionResult> UploadResultDocument(
      Guid caseId,
      IFormFile file)
    {
        var result = await _mediator.Send(
            new UploadCaseResultDocumentCommand(
                caseId,
                file));

        return Ok(result);
    }

}
public record ForwardInternalDto(string ToUserId, string? Remarks);
public record RemarksDto(string Remarks);

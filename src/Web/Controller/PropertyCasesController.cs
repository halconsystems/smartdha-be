using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class PropertyCasesController : BaseApiController
{
    private readonly IMediator _mediator;
    public PropertyCasesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateCaseCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    //[HttpPost("{caseId:guid}/submit")]
    //public async Task<IActionResult> Submit(Guid caseId, [FromBody] SubmitCaseCommandBody body, CancellationToken ct)
    //=> Ok(await _mediator.Send(new SubmitCaseCommand(caseId, body.ApplicantRemarks), ct));

    public record SubmitCaseCommandBody(string? ApplicantRemarks);

    //[HttpPost("{caseId:guid}/move")]
    //public async Task<IActionResult> Move(Guid caseId, [FromBody] MoveCaseBody body, CancellationToken ct)
    //=> Ok(await _mediator.Send(new MoveCaseCommand(caseId, body.Action, body.Remarks), ct));

    public record MoveCaseBody(StepAction Action, string? Remarks);


}


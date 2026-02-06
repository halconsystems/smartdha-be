using DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Commands;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Commands.RemoveProcessStep;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Queries.GetStepsByProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;


[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class WorkflowStepsController : BaseApiController
{
    private readonly IMediator _mediator;
    public WorkflowStepsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Add(AddProcessStepCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("by-process/{processId:guid}")]
    public async Task<IActionResult> ByProcess(Guid processId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetStepsByProcessQuery(processId), ct));

    [HttpDelete("{processId:guid}/steps/{stepId:guid}")]
    public async Task<IActionResult> RemoveStep(
    Guid processId,
    Guid stepId,
    CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RemoveProcessStepCommand(
                processId,
                stepId
            ),
            ct);

        return Ok(result);
    }

}

using DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcessPrerequisites;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessAllPrerequisite;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class ProcessReadController : BaseApiController
{
    private readonly IMediator _mediator;

    public ProcessReadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{processId:guid}/prerequisites")]
    public async Task<IActionResult> GetPrerequisites(
        Guid processId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetProcessAllPrerequisiteQuery(processId), ct);

        return Ok(result);
    }
}

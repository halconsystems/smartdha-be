using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.AddProcessPrerequisite;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreatePrerequisiteDefinition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class PrerequisitesController : BaseApiController
{
    private readonly IMediator _mediator;
    public PrerequisitesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("definitions")]
    public async Task<IActionResult> CreateDefinition(CreatePrerequisiteDefinitionCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("attach")]
    public async Task<IActionResult> Attach(AddProcessPrerequisiteCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));
}


using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.AddProcessPrerequisite;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreateAndAttachPrerequisite;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreatePrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.DeletePrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.UpdatePrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetAllPrerequisites;
using DHAFacilitationAPIs.Domain.Enums.PMS;
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

    [HttpPost]
    public async Task<IActionResult> CreateAndAttach(
        CreateAndAttachPrerequisiteCommand cmd,
        CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllPrerequisitesQuery(), ct));

    [HttpPut("definitions/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePrerequisiteDefinitionCommandBody body,
        CancellationToken ct)
        => Ok(await _mediator.Send(new UpdatePrerequisiteDefinitionCommand(
            id,
            body.Name,
            body.Code,
            body.Type,
            body.MinLength,
            body.MaxLength,
            body.AllowedExtensions
        ), ct));

    [HttpDelete("definitions/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeletePrerequisiteDefinitionCommand(id), ct));

    [HttpPost("definitions")]
    public async Task<IActionResult> CreateDefinition(CreatePrerequisiteDefinitionCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("attach")]
    public async Task<IActionResult> Attach(AddProcessPrerequisiteCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

}
public record UpdatePrerequisiteDefinitionCommandBody(
   string Name,
   string Code,
   PrerequisiteType Type,
   int? MinLength,
   int? MaxLength,
   string? AllowedExtensions
);


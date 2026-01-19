using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.AddProcessPrerequisite;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreateAndAttachPrerequisite;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreatePrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.DeletePrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.UpdatePrerequisiteDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetAllPrerequisites;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;
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

    [HttpGet("{processId:guid}/prerequisites")]
    public async Task<IActionResult> GetPrerequisites(
       Guid processId,
       CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetProcessPrerequisiteQuery(processId), ct);

        return Ok(result);
    }

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
         body.AllowedExtensions,
         body.Options
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
public class UpdatePrerequisiteDefinitionCommandBody
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public PrerequisiteType Type { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? AllowedExtensions { get; set; }

    // Default to empty list (never null)
    public List<PrerequisiteOptionInput>? Options { get; set; } = new();
}





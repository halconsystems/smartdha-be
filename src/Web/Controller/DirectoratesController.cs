using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Commands.CreateDirectorate;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Commands.DeleteDirectorate;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Commands.UpdateDirectorate;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class DirectoratesController : BaseApiController
{
    private readonly IMediator _mediator;
    public DirectoratesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateDirectorateCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _mediator.Send(new GetDirectoratesQuery(), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDirectorateCommandBody body,
        CancellationToken ct)
    {
        var cmd = new UpdateDirectorateCommand(id, body.Name, body.Code,body.ModuleId);
        return Ok(await _mediator.Send(cmd, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteDirectorateCommand(id), ct));
}
public record UpdateDirectorateCommandBody(
    string Name,
    string Code,
    Guid ModuleId
);

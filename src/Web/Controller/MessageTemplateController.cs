using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Commands.DeleteMessageTemplate;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Commands.PMSMessageTemplate;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Commands.UpdateMessageTemplate;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Queries.GetMessageTemplateById;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Queries.GetMessageTemplatesByModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class MessageTemplateController : BaseApiController
{
    private readonly IMediator _mediator;

    public MessageTemplateController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMessageTemplateCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateMessageTemplateCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMessageTemplateByIdQuery(id), ct));

    [HttpGet("module/{moduleId:guid}")]
    public async Task<IActionResult> GetByModule(Guid moduleId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMessageTemplatesByModuleQuery(moduleId), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteMessageTemplateCommand(id), ct));
}

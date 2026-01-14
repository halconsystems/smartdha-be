using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Commands.CreateServiceProcess;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries.GetProcessesByCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class ServiceProcessesController : BaseApiController
{
    private readonly IMediator _mediator;
    public ServiceProcessesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceProcessCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<IActionResult> ByCategory(Guid categoryId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetProcessesByCategoryQuery(categoryId), ct));
}


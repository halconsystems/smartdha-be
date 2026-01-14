using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.CreateServiceCategory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Queries.GetServiceCategories;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries.GetProcessesByCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class ServiceCategoriesController : BaseApiController
{
    private readonly IMediator _mediator;
    public ServiceCategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _mediator.Send(new GetServiceCategoriesQuery(), ct));

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<IActionResult> ByCategory(Guid categoryId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetProcessesByCategoryQuery(categoryId), ct));

    [HttpGet("{processId:guid}/prerequisites")]
    public async Task<IActionResult> GetPrerequisites(
        Guid processId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetProcessPrerequisiteQuery(processId), ct);

        return Ok(result);
    }
}

using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.CreateServiceCategory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.DeleteServiceCategory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.UpdateServiceCategory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Queries.GetServiceCategories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class ServiceCategoriesController : BaseApiController
{
    private readonly IMediator _mediator;
    public ServiceCategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceCategoryCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _mediator.Send(new GetServiceCategoriesQuery(), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateServiceCategoryRequest body,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateServiceCategoryCommand(id, body.Name, body.Code), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new DeleteServiceCategoryCommand(id), ct));
}

public record UpdateServiceCategoryRequest(
   string Name,
   string Code
);


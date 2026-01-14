using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.CreateServiceCategory;
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
}


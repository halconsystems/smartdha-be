using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeCategory;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeCategories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class PMSFeeCategoriesController : BaseApiController
{
    private readonly IMediator _mediator;

    public PMSFeeCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateFeeCategoryCommand cmd,
        CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _mediator.Send(new GetFeeCategoriesQuery(), ct));
}

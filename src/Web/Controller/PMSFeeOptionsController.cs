using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeOption;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeOptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class PMSFeeOptionsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PMSFeeOptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //[HttpPost]
    //public async Task<IActionResult> Create(
    //    CreateFeeOptionCommand cmd,
    //    CancellationToken ct)
    //    => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("{feeDefinitionId:guid}")]
    public async Task<IActionResult> Get(
        Guid feeDefinitionId,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new GetFeeOptionsQuery(feeDefinitionId), ct));
}

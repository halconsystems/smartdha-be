using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
using DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
using DHAFacilitationAPIs.Application.Feature.BowserCapacityRates.Commands;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class BowserCapacityRateController : ControllerBase
{
    private readonly IMediator _mediator;
    public BowserCapacityRateController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //Bowser Capacity Rates
    [HttpPost("Add"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddBowserCapacityRates([FromBody] AddBowserCapacityRatesCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Update"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> UpdateBowserCapacityRates([FromBody] UpdateBowserCapacityRateCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("Delete/{id}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBowserCapacityRateCommand { Id = id });
        return Ok(result);
    }

    //[HttpGet("Get"), AllowAnonymous]
    //public async Task<ActionResult<SuccessResponse<object>>> Get([FromQuery] Guid? id)
    //{
    //    var result = await _mediator.Send(new GetBowserCapacityRateQuery { Id = id });
    //    return Ok(result);
    //}



}


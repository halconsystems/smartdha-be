using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.CancelBowserRequest;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.CreateRequest;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.SubmitRequest;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetMyRequestById;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetMyRequests;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetPhaseCapacities;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetPhases;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.QuoteRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BowzerController : BaseApiController
{
    private readonly IMediator _mediator;
    public BowzerController(IMediator mediator) => _mediator = mediator;
 
    [HttpGet("GetPhases")]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> GetPhases(CancellationToken ct)
       => Ok(await _mediator.Send(new GetPhasesQuery(), ct));

    [HttpGet("{phaseId:guid}/capacities")]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> GetPhaseCapacities(Guid phaseId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPhaseCapacitiesQuery(phaseId), ct));

    [HttpPost("/api/quotes")]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> Quote([FromBody] QuoteRequestDto dto, CancellationToken ct)
       => Ok(await _mediator.Send(new QuoteRequestQuery(dto), ct));

    [HttpPost]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRequestCommand(dto), ct);
        return Ok(result);
    }

    // Submit
    [HttpPost("{id:guid}/submit")]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
        => Ok(new { status = await _mediator.Send(new SubmitRequestCommand(id), ct) });

    [HttpPost("cancel-request")]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> CancelBowserRequest([FromBody] CancelBowserRequestCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(new { message = result });
    }

    [HttpGet("history")]
    [ModuleAuthorize(Modules.Bowser)]
    public async Task<IActionResult> GetMyHistory(CancellationToken ct)
       => Ok(await _mediator.Send(new GetMyRequestsQuery(), ct));

    [HttpGet("MyRequest/{id:guid}")]
    public async Task<IActionResult> GetMyRequestById(string Id)
    {
        if (!Guid.TryParse(Id, out var requestId))
            return BadRequest("Invalid Request Id.");

        var result = await Mediator.Send(new GetMyRequestByIdQuery(requestId));

        return Ok(result);
    }
}

using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.CreateRequest;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Commands.SubmitRequest;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetMyRequests;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetPhaseCapacities;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetPhases;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.QuoteRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BowzerController : BaseApiController
{
    private readonly IMediator _mediator;
    public BowzerController(IMediator mediator) => _mediator = mediator;
 
    [HttpGet("GetPhases"),AllowAnonymous]
    public async Task<IActionResult> GetPhases(CancellationToken ct)
       => Ok(await _mediator.Send(new GetPhasesQuery(), ct));

    [HttpGet("{phaseId:guid}/capacities"),AllowAnonymous]
    public async Task<IActionResult> GetPhaseCapacities(Guid phaseId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPhaseCapacitiesQuery(phaseId), ct));

    [HttpPost("/api/quotes"), AllowAnonymous]
    public async Task<IActionResult> Quote([FromBody] QuoteRequestDto dto, CancellationToken ct)
       => Ok(await _mediator.Send(new QuoteRequestQuery(dto), ct));

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRequestCommand(dto), ct);
        return Ok(result);
    }

    // Submit
    [HttpPost("{id:guid}/submit"), AllowAnonymous]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
        => Ok(new { status = await _mediator.Send(new SubmitRequestCommand(id), ct) });

    [HttpGet("history"), AllowAnonymous]
    public async Task<IActionResult> GetMyHistory(CancellationToken ct)
       => Ok(await _mediator.Send(new GetMyRequestsQuery(), ct));
}

using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CancelMyPanic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicLocationUpdate;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicRequest;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyActivePanic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PanicController : BaseApiController
{
    private readonly IMediator _med;
    public PanicController(IMediator med) => _med = med;

    [HttpGet("emergency-types")]
    public Task<List<EmergencyTypeDto>> Types() => _med.Send(new GetEmergencyTypesQuery());

    public record CreateBody(Guid EmergencyTypeId, decimal Latitude, decimal Longitude, string? Notes, string? MediaUrl,string? MobileNumber);

    [HttpPost]
    public Task<PanicRequestDto> Create([FromBody] CreateBody b)
        => _med.Send(new CreatePanicRequestCommand(b.EmergencyTypeId, b.Latitude, b.Longitude, b.Notes, b.MediaUrl,b.MobileNumber));

    [HttpGet("my/active")]
    public Task<List<PanicRequestDto>> MyActive() => _med.Send(new GetMyActivePanicQuery());

    public record LocationBody(decimal Latitude, decimal Longitude, float? AccuracyMeters);
    [HttpPost("{id:guid}/location")]
    public Task<Guid> AddLocation(Guid id, [FromBody] LocationBody b)
        => _med.Send(new CreatePanicLocationUpdateCommand(id, b.Latitude, b.Longitude, b.AccuracyMeters));

    public record CancelBody(string? Remarks);
    [HttpPatch("{id:guid}/cancel")]
    public Task Cancel(Guid id, [FromBody] CancelBody b)
        => _med.Send(new CancelMyPanicCommand(id, b.Remarks));
}

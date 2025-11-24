using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CancelMyPanic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicLocationUpdate;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicRequest;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyActivePanic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PanicController : BaseApiController
{
    private readonly IMediator _med;
    public PanicController(IMediator med) => _med = med;

    [HttpGet("emergency-types")]
    [ModuleAuthorize(Modules.Panic)]
    public async Task<List<EmergencyTypeDto>> Types()
    {
        var list = await _med.Send(new GetEmergencyTypesQuery());
        return list.Where(x => x.IsActive==true).ToList();
    }

    public record CreateBody(Guid EmergencyTypeId, double Latitude, double Longitude, string? Notes, string? MediaUrl,string? MobileNumber);
    [HttpPost]
    [ModuleAuthorize(Modules.Panic)]
    public Task<PanicRequestDto> Create([FromBody] CreateBody b)
        => _med.Send(new CreatePanicRequestCommand(b.EmergencyTypeId, b.Latitude, b.Longitude, b.Notes, b.MediaUrl,b.MobileNumber));

    [HttpGet("my/active")]
    [ModuleAuthorize(Modules.Panic)]
    public Task<List<PanicRequestDto>> MyActive() => _med.Send(new GetMyActivePanicQuery());

    public record LocationBody(decimal Latitude, decimal Longitude, float? AccuracyMeters);
    [HttpPost("{id:guid}/location")]
    [ModuleAuthorize(Modules.Panic)]
    public Task<Guid> AddLocation(Guid id, [FromBody] LocationBody b)
        => _med.Send(new CreatePanicLocationUpdateCommand(id, b.Latitude, b.Longitude, b.AccuracyMeters));

    public record CancelBody(string? Remarks);
    [HttpPatch("{id:guid}/cancel")]
    [ModuleAuthorize(Modules.Panic)]
    public Task Cancel(Guid id, [FromBody] CancelBody b)
        => _med.Send(new CancelMyPanicCommand(id, b.Remarks));
}

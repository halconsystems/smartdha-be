using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Web.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("internal/realtime")]
[ApiExplorerSettings(GroupName = "panic")]
public class RealtimeController : ControllerBase
{
    private readonly IHubContext<PanicHub, IPanicHubClient> _hub;
    private readonly IConfiguration _cfg;
    public RealtimeController(IHubContext<PanicHub, IPanicHubClient> hub, IConfiguration cfg)
        => (_hub, _cfg) = (hub, cfg);

    [HttpPost("panic-created"),AllowAnonymous]
    public async Task<IActionResult> PanicCreated([FromBody] PanicCreatedRealtimeDto dto)
    {
        var secret = Request.Headers["X-RT-Secret"].ToString();
        if (string.IsNullOrWhiteSpace(secret) || secret != _cfg["Realtime:SharedSecret"])
            return Unauthorized();

        // Broadcast
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).PanicCreated(dto);
        //await _hub.Clients.All.SummaryChanged();
        return Ok();
    }
    
    [HttpPost("panic-update"), AllowAnonymous]
    public async Task<IActionResult> PanicUpdate([FromBody] PanicCreatedRealtimeDto dto)
    {
        var secret = Request.Headers["X-RT-Secret"].ToString();
        if (string.IsNullOrWhiteSpace(secret) || secret != _cfg["Realtime:SharedSecret"])
            return Unauthorized();

        // Broadcast
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).PanicUpdated(dto);
        //await _hub.Clients.All.SummaryChanged();
        return Ok();
    }
    [HttpPost("panic-statusupdate"), AllowAnonymous]
    public async Task<IActionResult> PanicStatusUpdate([FromBody] PanicUpdatedRealtimeDto dto)
    {
        var secret = Request.Headers["X-RT-Secret"].ToString();
        if (string.IsNullOrWhiteSpace(secret) || secret != _cfg["Realtime:SharedSecret"])
            return Unauthorized();

        // Broadcast
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).SendPanicUpdatedAsync(dto);
        //await _hub.Clients.All.SummaryChanged();
        return Ok();
    }
    [HttpPost("vehicle-locationupdate"), AllowAnonymous]
    public async Task<IActionResult> Vehiclelocationupdate([FromBody] UpdateLocation dto)
    {
        var secret = Request.Headers["X-RT-Secret"].ToString();
        if (string.IsNullOrWhiteSpace(secret) || secret != _cfg["Realtime:SharedSecret"])
            return Unauthorized();

        // Broadcast
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).VehicleLocationUpdate(dto);
        //await _hub.Clients.All.SummaryChanged();
        return Ok();
    }
}

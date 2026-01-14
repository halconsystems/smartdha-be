using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AcceptPanicDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CompletePanicDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicLocationUpdate;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicRequest;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicReview;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.ReachedPanicDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleLocation;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverLogin;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverProfile;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetCurrentUserPanicHistory;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyActivePanic;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyAssignedPanic;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyPanicHistory;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicles;
using DHAFacilitationAPIs.Application.Feature.User.Commands.DriverRefreshToken;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RefreshToken;
using DHAFacilitationAPIs.Application.Featurentity.Panic.Commands.CancelMyPanic;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "panic")]
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
    public Task<List<PanicRequestsDto>> MyActive() => _med.Send(new GetMyActivePanicQuery());

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

    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("Driver/Login")]
    public async Task<ActionResult> LoginDriver(DriverLoginCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Vehicles")]
    public async Task<ActionResult<List<SvVehicleListDto>>> GetVehicles()
    {
        var result = await _med.Send(new GetSvVehiclesforDriverQuery());
        return Ok(result);
    }

    [HttpPost("Drivers/AssignVehicle")]
    public async Task<ActionResult> AssignVehicle(AssignVehicleToDriverCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Driver/Panic/MyAssigned")]
    public async Task<ActionResult<PanicUpdatedRealtimeDto>> GetMyAssignedPanic()
    {
        var result = await _med.Send(new GetMyAssignedPanicQuery());
        return Ok(result);
    }


    [HttpPost("Driver/Dispatch/Accept")]
    public async Task<ActionResult<string>> AcceptDispatch(AcceptPanicDispatchCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPost("Driver/Panic/Reached")]
    public async Task<IActionResult> CompleteDispatch(ReachedPanicDispatchCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPost("Driver/Panic/Complete")]
    public async Task<IActionResult> CompleteDispatch(CompletePanicDispatchCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Driver/Panic/History")]
    public async Task<ActionResult<List<PanicUpdatedRealtimeDto>>> GetHistory()
    {
        var result = await _med.Send(new GetMyPanicHistoryQuery());
        return Ok(result);
    }


    [HttpPatch("Vehicles/{id:guid}/Update-location")]
    public async Task<IActionResult> UpdateVehicleLocation(Guid id, [FromBody] UpdateVehicleLocationRequest dto)
    {
        var result = await _med.Send(
            new UpdateSvVehicleLocationCommand(
                id,
                dto.Latitude,
                dto.Longitude
            )
        );
       
        return Ok(new
        {
            VehicleId = result,
            Message = "Vehicle location updated successfully."
        });
    }

    [HttpGet("User/Profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _med.Send(new GetUserProfileQuery());
        return Ok(result);
    }

    [HttpPost("User/Logout")]
    public async Task<IActionResult> Logout()
    {
        await _med.Send(new DriverLogoutCommand());
        return Ok(new { Message = "Logout successful" });
    }

    [AllowAnonymous]
    [EnableRateLimiting("AnonymousLimiter")]
    [HttpPost("User/refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] DriverRefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("panic/{panicId:guid}/review")]
    public async Task<IActionResult> CreateReview(Guid panicId, [FromBody] CreatePanicReviewRequest dto)
    {
        var id = await _med.Send(new CreatePanicReviewCommand(
            panicId,
            dto.Rating,
            dto.ReviewText
        ));

        return Ok(new { ReviewId = id, Message = "Review submitted successfully." });
    }

    [HttpGet("user-history")]
    [ModuleAuthorize(Modules.Panic)]
    public async Task<List<PanicHistoryDetailDto>> UserHistory()
    {
        var list = await _med.Send(new GetCurrentUserPanicHistoryQuery());
        return list;
    }
}

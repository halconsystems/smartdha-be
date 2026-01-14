using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AddPanicNote;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AddPanicResponder;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AssignPanic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AssignPanicToVehicle;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateEmergencyType;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateSvPoint;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateSvVehicle;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.DeleteEmergencyType;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.DeletePanicResponder;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.RegisterDriver;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.SendEmergencyAlert;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.SetEmergencyTypeStatus;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.SetSvPointStatus;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.SetSvVehicleStatus;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateEmergencyType;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdatePanicResponder;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdatePanicStatus;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvPoint;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicle;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleLocation;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleStatus;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllDrivers;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicRequests;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicResponders;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetDashboardSummary;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetDriverById;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetNearestVehicles;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicLogs;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicPaged;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicSummary;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicTrail;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvMapPoints;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvPoints;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicles;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicleStatusSummary;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicleSummary;
using DHAFacilitationAPIs.Application.Feature.User.Commands.DeleteDriver;
using DHAFacilitationAPIs.Application.Feature.User.Commands.SetDriverStatus;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateDriverInfo;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateDriverPassword;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DHAFacilitationAPIs.Application.Feature.Panic.PanicDto;

namespace DHAFacilitationAPIs.Web.Controller;


[ApiController]
[Route("api/panic")]
[ApiExplorerSettings(GroupName = "panic")]
public class PanicController : BaseApiController
{
    private readonly IMediator _med;
    public PanicController(IMediator med) => _med = med;

    [HttpGet("Dashboard")]
    public Task<DashboardSummaryDto> Dashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
         => _med.Send(new GetDashboardSummaryQuery(from, to));


    [HttpGet("GetAllPanic"),AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _med.Send(new GetAllPanicRequestsQuery(), ct);
        return Ok(result);
    }

    [HttpGet]
    public Task<List<PanicRequestListDto>> List(
        [FromQuery] int page = 1, [FromQuery] int size = 20,
        [FromQuery] int? code = null, [FromQuery] PanicStatus? status = null,
        [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
        [FromQuery] string? sort = null)
        => _med.Send(new GetPanicPagedQuery(page, size, code, status, from, to, sort));



    //[HttpGet("{id:guid}")]
    //public Task<PanicHistoryDetailDto> Get(Guid id) => _med.Send(new GetPanicHistoryByIdQuery(id));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPanicHistoryById(Guid id)
    {
        var result = await _med.Send(new GetPanicHistoryByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("{id:guid}/trail")]
    public Task<List<PanicTrailPointDto>> Trail(Guid id) => _med.Send(new GetPanicTrailQuery(id));

    [HttpGet("{id:guid}/logs")]
    public Task<List<PanicLogDto>> Logs(Guid id) => _med.Send(new GetPanicLogsQuery(id));

    [HttpGet("summary")]
    public Task<PanicSummaryDto> Summary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => _med.Send(new GetPanicSummaryQuery(from, to));

    public record UpdateStatusBody(PanicStatus NewStatus, string? AssignToUserId, string? Remarks);
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusBody b)
    {
        await _med.Send(new UpdatePanicStatusCommand(id, b.NewStatus, b.AssignToUserId, b.Remarks));
        return NoContent();
    }

    public record AssignBody(string AssignToUserId, string? Remarks);
    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignBody b)
    {
        await _med.Send(new AssignPanicCommand(id, b.AssignToUserId, b.Remarks));
        return NoContent();
    }

    public record NoteBody(string Note);
    [HttpPost("{id:guid}/note")]
    public async Task<IActionResult> AddNote(Guid id, [FromBody] NoteBody b)
    {
        await _med.Send(new AddPanicNoteCommand(id, b.Note));
        return NoContent();
    }

    [HttpPost("add-panic-responder"), AllowAnonymous]
    public async Task<IActionResult> AddPanicUser([FromBody] AddPanicResponderCommand command)
    {
        var result = await _med.Send(command);
        return Ok(new { PanicUserId = result });
    }

    [HttpPut("update-panic-responder"), AllowAnonymous]
    public async Task<IActionResult> UpdatePanicResponder([FromBody] UpdatePanicResponderCommand command)
    {
        await _med.Send(command);
        return Ok("Panic responder updated successfully.");
    }

    [HttpDelete("delete-panic-responder/{id:guid}"), AllowAnonymous]
    public async Task<IActionResult> DeletePanicResponder(Guid id)
    {
        var result = await _med.Send(new DeletePanicResponderCommand(id));
        return Ok(result);
    }

    [HttpGet("get-all-panic-responders"), AllowAnonymous]
    public async Task<IActionResult> GetAllPanicResponders([FromQuery] Guid? emergencyTypeId)
    {
        var result = await _med.Send(new GetAllPanicRespondersQuery(emergencyTypeId));
        return Ok(result);
    }

    [HttpGet("emergency-types")]
    public Task<List<EmergencyTypeDto>> Types() => _med.Send(new GetEmergencyTypesQuery());

    //Need to implement Create, Update, Delete Emergency Type endpoints below
    [HttpPost("emergencytypes-Create")]
    public async Task<IActionResult> Create(CreateEmergencyTypeCommand command)
    {
        var id = await _med.Send(command);
        return Ok(new { Message = "Emergency type created successfully", Id = id });
    }
   
    [HttpPut("emergencytypes-Update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmergencyTypeCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID in URL and body must match.");

        await _med.Send(command);
        return Ok(new { Message = "Emergency type updated successfully" });
    }

    [HttpPatch("emergencytypes/{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromQuery] bool isActive)
    {
        await _med.Send(new SetEmergencyTypeStatusCommand(id, isActive));

        return Ok(new
        {
            Message = isActive
                ? "Emergency type activated successfully"
                : "Emergency type deactivated successfully"
        });
    }

    [HttpPost("MarkLocation")]
    public async Task<ActionResult<Guid>> CreatePoint(CreateSvPointCommand cmd)
    {
        var id = await _med.Send(cmd);
        return Ok(id);
    }

    [HttpGet("GetLocations")]
    public async Task<ActionResult<List<SvPointDto>>> GetPoints()
    {
        var result = await _med.Send(new GetSvPointsQuery());
        return Ok(result);
    }

    // UPDATE POINT
    [HttpPut("MarkLocation/{id:guid}")]
    public async Task<IActionResult> UpdatePoint(Guid id, UpdateSvPointCommand cmd)
    {
        if (id != cmd.Id) return BadRequest("ID mismatch");
        await _med.Send(cmd);
        return NoContent();
    }

    // ACTIVATE/DEACTIVATE POINT
    [HttpPatch("MarkLocation/{id:guid}/status")]
    public async Task<IActionResult> SetPointStatus(Guid id, [FromQuery] bool isActive)
    {
        await _med.Send(new SetSvPointStatusCommand(id, isActive));

        return Ok(new
        {
            PointId = id,
            Message = isActive
                ? "Location activated successfully"
                : "Location deactivated successfully"
        });
    }

    [HttpPost("Vehicles")]
    public async Task<ActionResult<Guid>> CreateVehicle(CreateSvVehicleCommand cmd)
    {
        var id = await _med.Send(cmd);
        return Ok(id);
    }

    [HttpGet("Vehicles")]
    public async Task<ActionResult<List<SvVehicleListDto>>> GetVehicles()
    {
        var result = await _med.Send(new GetSvVehiclesQuery());
        return Ok(result);
    }

    // UPDATE VEHICLE
    [HttpPut("vehicles/{id:guid}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, UpdateSvVehicleCommand cmd)
    {
        if (id != cmd.Id) return BadRequest("ID mismatch");
        await _med.Send(cmd);
        return NoContent();
    }

    // ACTIVATE/DEACTIVATE VEHICLE
    [HttpPatch("vehicles/{id:guid}/status")]
    public async Task<IActionResult> SetVehicleStatus(Guid id, [FromQuery] bool isActive)
    {
        await _med.Send(new SetSvVehicleStatusCommand(id, isActive));

        return Ok(new
        {
            VehicleId = id,
            Message = isActive
                ? "Vehicle activated successfully"
                : "Vehicle deactivated successfully"
        });
    }

    [HttpPatch("Vehicles/{id:guid}/Update-Status")]
    public async Task<IActionResult> UpdateVehicleStatus(Guid id, [FromQuery] SvVehicleStatus status)
    {
        var result = await _med.Send(new UpdateSvVehicleStatusCommand(id, status));
        return Ok(new
        {
            VehicleId = result,
            Message = $"Vehicle status updated to {status}"
        });
    }

    [HttpGet("Vehicles/Summary")]
    public async Task<ActionResult<SvVehicleStatusSummaryDto>> GetVehicleStatusSummary()
    {
        var result = await _med.Send(new GetSvVehicleStatusSummaryQuery());
        return Ok(result);
    }

    [HttpGet("Map-Vehicle")]
    public async Task<ActionResult<List<SvMapPointDto>>> GetMapPoints()
    {
        var result = await _med.Send(new GetSvMapPointsQuery());
        return Ok(result);
    }

    [HttpGet("{panicId:guid}/Nearest-Vehicles")]
    public async Task<ActionResult<List<NearestVehicleDto>>> GetNearestVehicles(Guid panicId)
    {
        // fetch panic location
        var panic = await _med.Send(new GetPanicByIdQuery(panicId)); // you likely already have
        var result = await _med.Send(new GetNearestVehiclesQuery(
            panic.Latitude, panic.Longitude));
        return Ok(result);
    }

    [HttpGet("VehiclesMAP-Summary")]
    public async Task<ActionResult<SvVehicleSummaryDto>> GetVehicleSummary()
    {
        var result = await _med.Send(new GetSvVehicleSummaryQuery());
        return Ok(result);
    }

    //*************** Assign Panic ********************

    [HttpPost("Panic/{panicId:guid}/Assign")]
    public async Task<IActionResult> AssignPanic(
     Guid panicId,
     [FromBody] AssignPanicToVehicleRequest dto)
    {
        var dispatchId = await _med.Send(new AssignPanicToVehicleCommand(
            panicId,
            dto.VehicleId,
            dto.ControlRoomRemarks
        ));

        return Ok(new
        {
            DispatchId = dispatchId,
            Message = "Panic assigned to vehicle successfully."
        });
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

    [HttpPost("Drivers/Register")]
    public async Task<ActionResult> RegisterDriver(RegisterDriverCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Drivers")]
    public async Task<IActionResult> GetDrivers()
    {
        var result = await _med.Send(new GetAllDriversQuery());
        return Ok(result);
    }
    
    [HttpGet("Drivers/{id:guid}")]
    public async Task<IActionResult> GetDriver(Guid id)
    {
        var result = await _med.Send(new GetDriverByIdQuery(id));
        return Ok(result);
    }
   
    [HttpPut("Drivers/{id:guid}")]
    public async Task<IActionResult> UpdateDriver(Guid id, UpdateDriverInfoCommand cmd)
    {
        var updated = await _med.Send(cmd with { DriverId = id });
        return Ok(new { message = "Driver updated successfully" });
    }

    [HttpPut("Drivers/{id:guid}/Password")]
    public async Task<IActionResult> UpdateDriverPassword(Guid id, UpdateDriverPasswordCommand cmd)
    {
        await _med.Send(cmd with { DriverId = id });
        return Ok(new { message = "Password updated successfully." });
    }

    [HttpPatch("Drivers/{id:guid}/Status")]
    public async Task<IActionResult> SetDriverStatus(Guid id, bool isActive)
    {
        await _med.Send(new SetDriverStatusCommand(id, isActive));
        return Ok(new { message = "Driver status updated." });
    }

    [HttpDelete("Drivers/{id:guid}")]
    public async Task<IActionResult> DeleteDriver(Guid id)
    {
        await _med.Send(new DeleteDriverCommand(id));
        return Ok(new { message = "Driver deleted successfully." });
    }


    [HttpPost("send"),AllowAnonymous]
    public async Task<IActionResult> SendNotification(SendEmergencyAlertCommand command)
    {
        await _med.Send(command);
        return Ok(new { message = "Notification sent." });
    }
}

using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AddPanicNote;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AddPanicResponder;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.AssignPanic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.DeletePanicResponder;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdatePanicResponder;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdatePanicStatus;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicRequests;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicResponders;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetDashboardSummary;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicLogs;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicPaged;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicSummary;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicTrail;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DHAFacilitationAPIs.Application.Feature.Panic.PanicDto;

namespace DHAFacilitationAPIs.Web.Controller;


[ApiController]
[Route("api/panic")]
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



    [HttpGet("{id:guid}")]
    public Task<PanicDetailDto> Get(Guid id) => _med.Send(new GetPanicByIdQuery(id));

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
}

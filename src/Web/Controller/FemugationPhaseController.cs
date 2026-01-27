using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]
public class FemugationPhaseController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationPhaseController(IMediator med) => _med = med;

    [HttpGet("Get-Phases")]
    public async Task<List<PhaseDTO>> Phases()
    {
        var list = await _med.Send(new GetPhasesQuery());
        return list.ToList();
    }

    [HttpGet("Get-Phase-id")]
    public async Task<ActionResult<PhaseDTO>> PhasesById(Guid Id)
    {
        var list = await _med.Send(new GetPhaseByIdQuery(Id));
        return Ok(list);
    }

    [HttpPost("Create-Phase")]
    public async Task<ActionResult<string>> CreatePhase(AddPhasesCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPut("Update-Phase"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdatePhasesCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Phase"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeletePhase(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFemPhaseCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Phase"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active,ActiveInActivePhasesCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
}

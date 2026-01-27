using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]
public class FemugationSizeController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationSizeController(IMediator med) => _med = med;

    [HttpGet("Get-Size")]
    public async Task<List<TankerDTO>> Phases()
    {
        var list = await _med.Send(new GetAllTankerQuery());
        return list.ToList();
    }

    [HttpGet("Get-Size-id")]
    public async Task<ActionResult<TankerDTO>> PhasesById(Guid Id)
    {
        var list = await _med.Send(new GetTankerByIdQuery(Id));
        return Ok(list);
    }

    [HttpPost("Create-Size")]
    public async Task<ActionResult<string>> CreatePhase(AddTankerCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPut("Update-Size"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdateTankerCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Size"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeletePhase(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteTankerCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Size"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveSizeCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
}


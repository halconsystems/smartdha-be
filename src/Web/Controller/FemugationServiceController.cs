using DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.DeleteService;
using DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServiceById;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]
public class FemugationServiceController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationServiceController(IMediator med) => _med = med;

    [HttpGet("Get-Service")]
    public async Task<List<ServiceDTO>> Phases()
    {
        var list = await _med.Send(new GetAllServicesQuery());
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpGet("Get-Service-id")]
    public async Task<ActionResult<ServiceDTO>> PhasesById(Guid Id)
    {
        var list = await _med.Send(new GetAllServicesByIdQuery(Id));
        return Ok(list);
    }

    [HttpPost("Create-Service")]
    public async Task<ActionResult<string>> CreatePhase(AddServicesCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPut("Update-Service"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdateServiceCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Service"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeletePhase(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFemPhaseCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Service"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveServiceCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
}

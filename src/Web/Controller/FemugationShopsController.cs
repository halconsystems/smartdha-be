using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;
using DHAFacilitationAPIs.Application.Feature.Shops.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]

public class FemugationShopsController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationShopsController(IMediator med) => _med = med;

    [HttpGet("Get-Shops")]
    public async Task<List<FMShopsDTO>> Phases()
    {
        var list = await _med.Send(new GetFMShopsQuery());
        return list.ToList();
    }

    [HttpGet("Get-Shops-id")]
    public async Task<ActionResult<TankerDTO>> PhasesById(Guid Id)
    {
        var list = await _med.Send(new GetFMShopsByIdQuery(Id));
        return Ok(list);
    }

    [HttpPost("Create-Shop")]
    public async Task<ActionResult<string>> CreatePhase(CreateFMShopsCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPut("Update-Shop"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdateFMShopsCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Shop"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeletePhase(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFMShopsCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Shop"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveFMShopCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
}


using DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]

public class FemugationTaxDiscountController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationTaxDiscountController(IMediator med) => _med = med;

    [HttpGet("Get-Discount/Tax")]
    public async Task<List<FemigationDTdTO>> Phases()
    {
        var list = await _med.Send(new GetAllFMDiscountQuery());
        return list.ToList();
    }

    [HttpGet("Get-Discount/Tax-id")]
    public async Task<ActionResult<FemigationDTdTO>> PhasesById(Guid Id)
    {
        var list = await _med.Send(new GetFmDTByIdQuery(Id));
        return Ok(list);
    }

    [HttpPost("Create-Discount/Tax")]
    public async Task<ActionResult<string>> CreatePhase(CreateFMShopsCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPut("Update-Discount/Tax"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdateFemDTCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Discount/Tax"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeletePhase(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFMDTCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Discount/Tax"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveFMDTCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
}



using DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CompletePanicDispatch;
using Microsoft.AspNetCore.Mvc;
using static DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries.FemugationDashBoardDTO;
using static DHAFacilitationAPIs.Application.Feature.Orders.Queries.OrderDashBoardDTO;

namespace DHAFacilitationAPIs.Web.Controller;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]
public class FemugationProcessController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationProcessController(IMediator med) => _med = med;

    [HttpPost("Shop/Femugation/Process")]
    public async Task<IActionResult> FemugationProcess(FemugationProcess cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Get-FemugationList")]
    public async Task<List<FemugationDTO>> FemlIST()
    {
        var list = await _med.Send(new GetAllFemugationListQuery());
        return list.ToList();
    }

    [HttpGet("Get-FemugationHistory")]
    public async Task<ActionResult<FemugationDTO>> FemHistory(Guid Id)
    {
        var result = await _med.Send(new FemugationHistoryQuery(Id));
        return Ok(result);
    }

    [HttpGet("Dashboard")]
    public Task<FemugationDashboardSummaryDto> Dashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => _med.Send(new GetFemugationDashboardSummaryQuery(from, to));

    [HttpPost("Create-Femugation")]
    public async Task<ActionResult<string>> CreateFemugation(AddFemugationCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpPost("Shop/Femugation/MobileProcess")]
    public async Task<IActionResult> FemugationMobileProcess(FemugationProcess cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

}

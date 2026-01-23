using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CompletePanicDispatch;
using Microsoft.AspNetCore.Mvc;

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
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpGet("Get-FemugationHistory")]
    public async Task<ActionResult<FemugationDTO>> FemHistory(Guid Id)
    {
        var result = await _med.Send(new FemugationHistoryQuery(Id));
        return Ok(result);
    }
}

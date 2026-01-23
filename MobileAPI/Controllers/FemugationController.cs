using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CompletePanicDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Femugation")]

public class FemugationController : BaseApiController
{
    private readonly IMediator _med;
    public FemugationController(IMediator med) => _med = med;

    [HttpGet("Get-Services")]
    public async Task<List<ServiceDTO>> Types()
    {
        var list = await _med.Send(new GetAllServicesQuery());
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpGet("Get-Phases")]
    public async Task<List<PhaseDTO>> Phases()
    {
        var list = await _med.Send(new GetPhasesQuery());
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpGet("Get-Size")]
    public async Task<List<TankerDTO>> Size()
    {
        var list = await _med.Send(new GetAllTankerQuery());
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpGet("Get-Shops")]
    public async Task<List<FMShopsDTO>> Shops()
    {
        var list = await _med.Send(new GetFMShopsQuery());
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpGet("Get-Discount-Taxes")]
    public async Task<List<FemigationDTdTO>> DiscounrTax()
    {
        var list = await _med.Send(new GetAllFMDiscountQuery());
        return list.Where(x => x.IsActive == true).ToList();
    }

    [HttpPost("Create-Femugation")]
    public async Task<ActionResult<string>> CreateFemugation(AddFemugationCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

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

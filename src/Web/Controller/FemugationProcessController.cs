using DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Command;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.Panic.Commands.CompletePanicDispatch;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
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


    #region Fumigation Phase Crud Here

    [HttpPost("Create-Phase")]
    [Tags("01 - Phase")]
    public async Task<ActionResult<string>> CreatePhase(AddPhasesCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }
    [HttpGet("Get-Phases")]
    [Tags("01 - Phase")]
    public async Task<List<PhaseDTO>> Phases()
    {
        var list = await _med.Send(new GetPhasesQuery());
        return list.ToList();
    }

    [HttpGet("Get-PhaseDetails-By-Id")]
    [Tags("01 - Phase")]
    public async Task<ActionResult<PhaseDTO>> PhasesById(Guid Id)
    {
        var list = await _med.Send(new GetPhaseByIdQuery(Id));
        return Ok(list);
    }
    [HttpPut("Update-Phase")]
    [Tags("01 - Phase")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdatePhasesCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Phase")]
    [Tags("01 - Phase")]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeletePhase(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFemPhaseCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Phase"), AllowAnonymous]
    [Tags("01 - Phase")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActivePhasesCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    #endregion

    #region Femugation Service Crud Here
    [HttpPost("Create-Service")]
    [Tags("02 - Service")]
    public async Task<ActionResult<string>> CreatePhase(AddServicesCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }
    [HttpGet("Get-Service")]
    [Tags("02 - Service")]
    public async Task<List<ServiceDTO>> Services()
    {
        var list = await _med.Send(new GetAllServicesQuery());
        return list.ToList();
    }

    [HttpGet("Get-Service-By-id")]
    [Tags("02 - Service")]
    public async Task<ActionResult<ServiceDTO>> ServicesById(Guid Id)
    {
        var list = await _med.Send(new GetAllServicesByIdQuery(Id));
        return Ok(list);
    }

    [HttpPut("Update-Service"), AllowAnonymous]
    [Tags("02 - Service")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateService([FromBody] UpdateServiceCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Service"), AllowAnonymous]
    [Tags("02 - Service")]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeleteService(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFemPhaseCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Service"), AllowAnonymous]
    [Tags("02 - Service")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateService(bool Active, ActiveInActiveServiceCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
    #endregion

    #region Femigation Size Crud Here
    [HttpPost("Create-Size")]
    [Tags("03 - Size")]
    public async Task<ActionResult<string>> CreateSize(AddTankerCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Get-Size")]
    [Tags("03 - Size")]
    public async Task<List<TankerDTO>> Sizes()
    {
        var list = await _med.Send(new GetAllTankerQuery());
        return list.ToList();
    }

    [HttpGet("Get-Size-id")]
    [Tags("03 - Size")]
    public async Task<ActionResult<TankerDTO>> SizeById(Guid Id)
    {
        var list = await _med.Send(new GetTankerByIdQuery(Id));
        return Ok(list);
    }

    [HttpPut("Update-Size"), AllowAnonymous]
    [Tags("03 - Size")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateSize([FromBody] UpdateTankerCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Size"), AllowAnonymous]
    [Tags("03 - Size")]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeleteSize(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteTankerCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Size"), AllowAnonymous]
    [Tags("03 - Size")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveSizeCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
    #endregion

    #region Femigation Tax Dicount Crud Here
    [HttpPost("Create-Discount/Tax")]
    [Tags("04 - Tax-Discount")]
    public async Task<ActionResult<string>> CreatePhase(AddFemigationDTCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Get-Discount/Tax")]
    [Tags("04 - Tax-Discount")]
    public async Task<List<FemigationDTdTO>> TaxDiscount()
    {
        var list = await _med.Send(new GetAllFMDiscountQuery());
        return list.ToList();
    }

    [HttpGet("Get-Discount/Tax-id")]
    [Tags("04 - Tax-Discount")]
    public async Task<ActionResult<FemigationDTdTO>> TaxDiscountById(Guid Id)
    {
        var list = await _med.Send(new GetFmDTByIdQuery(Id));
        return Ok(list);
    }
    [HttpPut("Update-Discount/Tax"), AllowAnonymous]
    [Tags("04 - Tax-Discount")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase([FromBody] UpdateFemDTCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Discount/Tax"), AllowAnonymous]
    [Tags("04 - Tax-Discount")]
    public async Task<ActionResult<SuccessResponse<Guid>>> Deletetaxdiscount(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFMDTCommand(id), ct);
        return StatusCode(result.Status, result);
    }


    [HttpPut("Active/InAtive-Discount/Tax"), AllowAnonymous]
    [Tags("04 - Tax-Discount")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveFMDTCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    #endregion

    #region Femigation Shops Crud Here
    [HttpPost("Create-Shop")]
    [Tags("05 - FemigationShops")]
    public async Task<ActionResult<string>> CreatePhase(CreateFMShopsCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Get-Shops")]
    [Tags("05 - FemigationShops")]
    public async Task<List<FMShopsDTO>> Shops()
    {
        var list = await _med.Send(new GetFMShopsQuery());
        return list.ToList();
    }

    [HttpGet("Get-Shops-id")]
    [Tags("05 - FemigationShops")]
    public async Task<ActionResult<TankerDTO>> ShopById(Guid Id)
    {
        var list = await _med.Send(new GetFMShopsByIdQuery(Id));
        return Ok(list);
    }
    [HttpPut("Update-Shop"), AllowAnonymous]
    [Tags("05 - FemigationShops")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateShop([FromBody] UpdateFMShopsCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Shop"), AllowAnonymous]
    [Tags("05 - FemigationShops")]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeleteShop(Guid id, CancellationToken ct)
    {
        var result = await _med.Send(new DeleteFMShopsCommand(id), ct);
        return StatusCode(result.Status, result);
    }

    [HttpPut("Active/InAtive-Shop"), AllowAnonymous]
    [Tags("05 - FemigationShops")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdatePhase(bool Active, ActiveInActiveFMShopCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }
    #endregion


    #region Femigation Order Process
    [HttpPost("Shop/Femugation/Process")]
    [Tags("06 - FemigationShopProcess")]
    public async Task<IActionResult> FemugationProcess(FemugationProcess cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Get-Femugation-Order-List")]
    [Tags("06 - FemigationShopProcess")]
    public async Task<List<FemugationDTO>> FemlIST()
    {
        var list = await _med.Send(new GetAllFemugationListQuery());
        return list.ToList();
    }

    [HttpGet("Get-Femugation-Order-History")]
    [Tags("06 - FemigationShopProcess")]
    public async Task<ActionResult<FemugationDTO>> FemHistory(Guid Id)
    {
        var result = await _med.Send(new FemugationHistoryQuery(Id));
        return Ok(result);
    }

    [HttpGet("Femigation-Dashboard")]
    [Tags("06 - FemigationShopProcess")]
    public Task<FemugationDashboardSummaryDto> Dashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => _med.Send(new GetFemugationDashboardSummaryQuery(from, to));
    #endregion


    #region Femigation Creation From Web
    [HttpPost("Create-Femugation")]
    [Tags("07 - FemigationCreation")]
    public async Task<ActionResult<string>> CreateFemugation(AddFemugationCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }
    #endregion

    #region Femigation Process From Mobile
    [HttpPost("Shop/Femugation/MobileProcess")]
    [Tags("07 - FemigationMobileProcess")]
    public async Task<IActionResult> FemugationMobileProcess(FemugationProcess cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }
    #endregion

    #region Femigation Shop Tax Dicount Crud Here
    [HttpPost("Create-Shop-Discount/Tax")]
    [Tags("8 - Shop-Tax-Discount")]
    public async Task<ActionResult<string>> CreateShopTaxDiscounr(CreateAddFemShopDTCommand cmd)
    {
        var result = await _med.Send(cmd);
        return Ok(result);
    }

    [HttpGet("Get-Shop-Discount/Tax")]
    [Tags("8 - Shop-Tax-Discount")]
    public async Task<List<FemDTSettingDTO>> ShopTaxDiscount()
    {
        var list = await _med.Send(new GetAllFemShopDTQuery());
        return list.ToList();
    }
    [HttpGet("Get-Shop-Discount/Tax-ByShopId")]
    [Tags("8 - Shop-Tax-Discount")]
    public async Task<List<FemDTSettingDTO>> ShopTaxDiscountByShopid(Guid ShopId)
    {
        var list = await _med.Send(new GetFemDTBYShopIdQuery(ShopId));
        return list.ToList();
    }

    [HttpGet("Get-Shop-Discount/Tax-id")]
    [Tags("8 - Shop-Tax-Discount")]
    public async Task<ActionResult<FemDTSettingDTO>> ShopTaxDiscountById(Guid Id)
    {
        var list = await _med.Send(new GetFemShopDTbyIdQuery(Id));
        return Ok(list);
    }
    [HttpPut("Update-Shop-Discount/Tax"), AllowAnonymous]
    [Tags("8 - Shop-Tax-Discount")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateShopPhase([FromBody] UpdateFemigationShopDTCommand cmd, CancellationToken ct)
    {
        var result = await _med.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

  

    #endregion


}

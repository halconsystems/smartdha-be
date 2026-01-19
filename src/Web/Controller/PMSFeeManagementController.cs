using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.AddFeeSlab;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CalculateCaseFee;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeDefinitionWithOptions;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.GenerateVoucherByCase;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeByProcess;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeDefinition;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeDefinitionByProcessId;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeSlabs;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class PMSFeeManagementController : BaseApiController
{
    private readonly IMediator _mediator;
    public PMSFeeManagementController(IMediator mediator) => _mediator = mediator;

    //[HttpPost("definitions")]
    //public async Task<IActionResult> CreateDefinition(CreateFeeDefinitionCommand cmd, CancellationToken ct)
    //    => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        CreateFeeDefinitionWithOptionsCommand cmd,
        CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("by-process/{processId:guid}")]
    public async Task<IActionResult> GetByProcess(
        Guid processId,
        CancellationToken ct)
    {
        return Ok(await _mediator.Send(
            new GetFeeDefinitionByProcessIdQuery(processId), ct));
    }

    //[HttpPost("slabs")]
    //public async Task<IActionResult> AddSlab(AddFeeSlabCommand cmd, CancellationToken ct)
    //    => Ok(await _mediator.Send(cmd, ct));

    //[HttpGet("definitions/{feeDefinitionId:guid}")]
    //public async Task<IActionResult> GetDefinition(Guid feeDefinitionId, CancellationToken ct)
    //    => Ok(await _mediator.Send(new GetFeeDefinitionQuery(feeDefinitionId), ct));

    //[HttpGet("slabs/{feeDefinitionId:guid}")]
    //public async Task<IActionResult> GetSlabs(Guid feeDefinitionId, CancellationToken ct)
    //    => Ok(await _mediator.Send(new GetFeeSlabsQuery(feeDefinitionId), ct));

    //[HttpGet("processes/{processId:guid}/fees")]
    //public async Task<IActionResult> GetByProcess(Guid processId, CancellationToken ct)
    //    => Ok(await _mediator.Send(new GetFeeByProcessQuery(processId), ct));

//    [HttpPost("cases/{caseId:guid}/calculate-fee")]
//    public async Task<IActionResult> CalculateFee(
//        Guid caseId,
//        [FromBody] CalculateCaseFeeRequest body,
//        CancellationToken ct)
//        => Ok(await _mediator.Send(
//            new CalculateCaseFeeCommand(caseId, body.PropertyArea, body.AreaUnit), ct));

//    [HttpPost("finance/vouchers/generate-by-case")]
//    public async Task<IActionResult> GenerateVoucher(
//        [FromBody] GenerateVoucherByCaseCommand cmd,
//        CancellationToken ct)
//        => Ok(await _mediator.Send(cmd, ct));

//    public record CalculateCaseFeeRequest(
//    decimal? PropertyArea,
//    AreaUnit? AreaUnit
//);
}

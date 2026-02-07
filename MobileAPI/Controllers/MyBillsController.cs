using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetAllBills;
using DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetAllBillsByConsumerId;
using DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetMyBillDetail;
using DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetMyPaidBills;
using DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.BillInquiry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "mybills")]
public class MyBillsController : BaseApiController
{
    private readonly IMediator _mediator;
    public MyBillsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("BillInquiry")]
    public async Task<IActionResult> BillInquiry()
    {
        var query = new GetAllBillsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("BillInquiry/{consumerNo}")]
    public async Task<IActionResult> BillInquiry(string consumerNo)
    {
        var query = new GetAllBillsByConsumerId(consumerNo);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("my-bills/paid")]
    public async Task<IActionResult> GetMyPaidBills(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyPaidBillsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("my-bills/{paymentBillId:guid}")]
    public async Task<IActionResult> GetMyBillDetail(
    Guid paymentBillId,
    CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetMyBillDetailQuery(paymentBillId), ct);

        return Ok(result);
    }
}

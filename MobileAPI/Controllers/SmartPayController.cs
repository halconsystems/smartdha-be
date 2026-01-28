using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.SmartPay.Commands.UploadBill;
using DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.BillInquiry;
using DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.ConsumerHistory;
using DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.ConsumerInquiry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SmartPayController : BaseApiController
{
    private readonly IMediator _mediator;

    public SmartPayController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ConsumerInquiry/{cellNo}"),AllowAnonymous]
   // [ModuleAuthorize(Modules.MyBills)]
    public async Task<IActionResult> ConsumerInquiry(string cellNo)
    {
        var query = new ConsumerInquiryQuery { CellNo = cellNo };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("BillInquiry/{consumerNo}"),AllowAnonymous]
    //[ModuleAuthorize(Modules.MyBills)]
    public async Task<IActionResult> BillInquiry(string consumerNo)
    {
        var query = new BillInquiryQuery { ConsumerNo = consumerNo };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    //[HttpGet("ConsumerHistory/{consumerNo}"), AllowAnonymous]
    //public async Task<IActionResult> ConsumerHistory(string consumerNo)
    //{
    //    var query = new ConsumerHistoryQuery { ConsumerNo = consumerNo };
    //    var result = await _mediator.Send(query);
    //    return Ok(result);
    //}
    [HttpGet("MyBillInquiry"),AllowAnonymous]
    //[ModuleAuthorize(Modules.MyBills)]
    public async Task<IActionResult> MyConsumerInquiry()
    {
        var cellNo = "923222781985";
        var query = new ConsumerInquiryQuery { CellNo = cellNo };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("MyHistory")]
    [ModuleAuthorize(Modules.MyBills)]
    public async Task<IActionResult> MyHistory()
    {
        var consumerNo = "600214081";

        if (string.IsNullOrWhiteSpace(consumerNo))
            throw new UnAuthorizedException("Consumer number missing in token.");

        var query = new ConsumerHistoryQuery { ConsumerNo = consumerNo };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    //[HttpPost("UploadBill"), AllowAnonymous]
    //public async Task<IActionResult> UploadBill([FromBody] SmartPayUploadBillRequest model)
    //{
    //    var command = new UploadBillCommand { Request = model };
    //    var result = await _mediator.Send(command);
    //    return Ok(result);
    //}
}


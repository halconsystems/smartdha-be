using DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetAllBills;
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

}

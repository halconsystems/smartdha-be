using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.CreateVisitorPass;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.DeleteVisitorPass;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.UpdateVisitorPass;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassGroupedQuery;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassPdfQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class VisitorPassController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;

    public VisitorPassController(IMediator mediator, IUser loggedInUser)
    {
        _mediator = mediator;
        _loggedInUser = loggedInUser;
    }

    [HttpPost("add-visitorpass"), AllowAnonymous]
    public async Task<IActionResult> Create(CreateVisitorPassCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("update-visitorpass"), AllowAnonymous]
    public async Task<IActionResult> Update(Guid id, UpdateVisitorPassCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("delete-visitorpass"), AllowAnonymous]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteVisitorPassCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetVisitorPassByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("get-all-visitors"), AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetVisitorPassGroupedQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("download-pdf"), AllowAnonymous]
    public async Task<IActionResult> GetPdf(GetVisitorPassPdfQuery request)
    {
        var result = await _mediator.Send(request);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(ApiResult<string>.Ok(result.Data!, "PDF URL fetched successfully"));
    }

}

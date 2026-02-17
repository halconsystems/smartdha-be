using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.CreateLaggagePass;
using DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.DeleteLaggagePass;
using DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.UpdateLaggagePass;
using DHAFacilitationAPIs.Application.Feature.LaggagePass.Queries.GetLaggagePassById;
using DHAFacilitationAPIs.Application.Feature.LaggagePass.Queries.GetLaggagePassByList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class LaggagePassController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;

    public LaggagePassController(IUser loggedInUser,IMediator mediator)
    {
        _mediator = mediator;
        _loggedInUser = loggedInUser;
    }

    [HttpPost("create-laggagepass"), AllowAnonymous]
    public async Task<IActionResult> Create(CreateLaggagePassCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("update-laggagepass"), AllowAnonymous]
    public async Task<IActionResult> Update(Guid id, UpdateLaggagePassCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("delete-laggagepass"), AllowAnonymous]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteLaggagePassCommand { Id = id };
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetLaggagePassByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("getall"), AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllLaggagePassQuery();
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}

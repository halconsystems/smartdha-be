using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.CreateLuggagePass;
using DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.DeleteLuggagePass;
using DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.UpdateLuggagePass;
using DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetAllLuggagePass;
using DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetLuggagePassById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class LuggagePassController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;

    public LuggagePassController(IUser loggedInUser,IMediator mediator)
    {
        _mediator = mediator;
        _loggedInUser = loggedInUser;
    }

    [HttpPost("create-luggagepass"), AllowAnonymous]
    public async Task<IActionResult> Create(CreateLuggagePassCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("update-luggagepass"), AllowAnonymous]
    public async Task<IActionResult> Update(Guid id, UpdateLuggagePassCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("delete-luggagepass"), AllowAnonymous]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteLuggagePassCommand { Id = id };
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetLuggagePassByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("getall"), AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllLuggagePassQuery();
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}

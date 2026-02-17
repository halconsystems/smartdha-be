using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;
using DHAFacilitationAPIs.Application.Feature.Worker.Commands.AddWorker;
using DHAFacilitationAPIs.Application.Feature.Worker.Commands.UpdateWorker;
using DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetAllWorkers;
using DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetWorkerById;
using DHAFacilitationAPIs.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class WorkerController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    public WorkerController(IUser loggedInUser, IMediator mediator)
    {
        _loggedInUser = loggedInUser;
        _mediator = mediator;
    }

    [HttpPost("add-worker"), Authorize(Roles = AllRoles.Member)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddWorker([FromForm] AddWorkerCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResult<Guid>.Ok(result.Data, "Worker added successfully"));
    }

    [HttpPost("update-worker"), Authorize(Roles = AllRoles.Member)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateWorker([FromForm] UpdateWorkerCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResult<UpdateWorkerResponse>.Ok(result.Data!, "Worker updated successfully"));
    }

    [HttpGet("get-all-workers")]
    [Authorize(Roles = AllRoles.Member)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllWorkerQuery());
        return Ok(ApiResult<List<GetAllWorkerQueryResponse>>.Ok(result.Data!.ToList(), "Record fetched successfully"));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = AllRoles.Member)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetWorkerByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        if (!result.Succeeded)
            return BadRequest(ApiResult<string>.Fail(result.Errors.First()));

        return Ok(ApiResult<GetWorkerByIdResponse>.Ok(result.Data!, "Record fetched successfully"));
    }
}

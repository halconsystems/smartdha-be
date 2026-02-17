using Azure.Core;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.Property.Queries;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle.UpdateVecleCommandHandler;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class VehicleController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    public VehicleController(IUser loggedInUser, IMediator mediator)
    {
        _loggedInUser = loggedInUser;
        _mediator = mediator;
    }
    [HttpPost("add-vehicle"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(CreateVehicleCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<IActionResult> GetVehicle(Guid id)
    {
        var result = await _mediator.Send(new GetVehicleByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost("update-vehicle"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update([FromForm] UpdateVehicleCommand request)
    {
        return Ok(await _mediator.Send(request));
    }
 
    [HttpPost("delete"), AllowAnonymous]
    public async Task<IActionResult> Delete([FromBody] DeleteVehicleCommand request)
    {
        return Ok(await _mediator.Send(request));

    }
}

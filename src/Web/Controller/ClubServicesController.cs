using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
using DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.CreateService;
using DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.DeleteService;
using DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.UpdateService;
using DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServiceById;
using DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServices;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class ClubServicesController : BaseApiController
{
    private readonly IMediator _mediator;
    public ClubServicesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateServiceCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateServiceCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, bool hardDelete, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteServiceCommand(id, hardDelete), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DHAFacilitationAPIs.Domain.Entities.Services?>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetServiceByIdQuery(id), ct));

    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<List<DHAFacilitationAPIs.Domain.Entities.Services>>> GetAll([FromQuery] bool includeInactive, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetServicesQuery(includeInactive, page, pageSize), ct));
}

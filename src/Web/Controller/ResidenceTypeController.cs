using DHAFacilitationAPIs.Application.Feature.CreateResidenceType.Queries.GetResidenceTypes;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.CreateResidenceType;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.DeleteResidenceType;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.UpdateResidenceType;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries.GetResidenceTypeById;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class ResidenceTypeController : BaseApiController
{
    private readonly IMediator _mediator;
    public ResidenceTypeController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateResidenceTypeCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateResidenceTypeCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteResidenceTypeCommand(id), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResidenceTypeDto?>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetResidenceTypeByIdQuery(id), ct));

    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<List<ResidenceTypeDto>>> GetAll()
        => Ok(await _mediator.Send(new GetResidenceTypesQuery()));
}

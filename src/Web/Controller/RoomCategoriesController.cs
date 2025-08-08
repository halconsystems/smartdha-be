using DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.CreateRoomCategory;
using DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.DeleteRoomCategory;
using DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.UpdateRoomCategory;
using DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategories;
using DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategoryById;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class RoomCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RoomCategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateRoomCategoryCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateRoomCategoryCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, bool hardDelete, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteRoomCategoryCommand(id, hardDelete), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DHAFacilitationAPIs.Domain.Entities.RoomCategory?>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetRoomCategoryByIdQuery(id), ct));

    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<DHAFacilitationAPIs.Domain.Entities.Services>>> GetAll([FromQuery] bool includeInactive, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetRoomCategoriesQuery(includeInactive, page, pageSize), ct));
}

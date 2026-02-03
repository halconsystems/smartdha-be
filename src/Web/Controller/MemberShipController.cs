using DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Command;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Command;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;
using DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Queries.GetAllMembershipPurposes;
using DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Command;
using DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;
using DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;
using DHAFacilitationAPIs.Application.Feature.Religion.Command;
using DHAFacilitationAPIs.Application.Feature.Religion.Queries;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Command;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries.FemugationDashBoardDTO;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "MemberShip")]
public class MemberShipController : BaseApiController
{
    private readonly IMediator _mediator;
    public MemberShipController(IMediator med) => _mediator = med;

    [HttpPost("Create-MemberShip"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateMemberShipCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateMemberShipCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-MemberShips"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShips(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllMemberShipQuery(), ct);
        return Ok(result);
    }

    [HttpPost("Create-MemberShipCategories"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateMemberShipCategories(CreateMemberShipCategoryCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("UpdateCategory")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateMemberShipCategories(Guid id, UpdateMemberShipCategoryCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-MemberShipCategories"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShipsCategories(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllMemberShipCategoryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-MemberShipCategoriesById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShipsCategoriesById(Guid MemberShipId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMemberShipCategoryByIdQuery(MemberShipId), ct);
        return Ok(result);
    }

    [HttpPost("Create-Religon"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateReligon(CreateReligionCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-Religon")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateReligon(Guid id, UpdateReligionCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-Religons"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligon(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllReligonCategoryQuery(), ct);
        return Ok(result);
    }

    [HttpPost("Create-ReligonSect"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateReligonSect(CreateReligonSectCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-ReligonSect")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateReligonSect(Guid id, UpdateReligonSectCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-ReligonSect"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSect(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllReligonSectCategoryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-ReligonSect-ById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSectById(Guid ReligonId,CancellationToken ct)
    {
        var result = await _mediator.Send(new GetReligonSectByIdQuery(ReligonId), ct);
        return Ok(result);
    }

    [HttpGet("get-MemberShipRequestLisr"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShipsRequest(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMemberRequestListQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-MemberShipRequestById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetMemberShipRequestByIdId(Guid MemberShipId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMemberShipHistoryQuery(MemberShipId), ct);
        return Ok(result);
    }

    [HttpGet("Dashboard")]
    public Task<MemberVerificationDashboardDto> Dashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => _mediator.Send(new GetMemberShipDashboardSummaryQuery(from, to));

    [HttpPost("Update-member-requests")]
    public async Task<IActionResult> UpdateNonMemberRequests(UpdateMemberShipRequestCommand updateNonMemberVerificationCommand)
    {
        var result = await Mediator.Send(updateNonMemberVerificationCommand);
        return Ok(result);

    }
}

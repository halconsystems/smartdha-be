using Azure.Core;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;
using DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Commands.AddMembershipPurpose;
using DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Queries.GetAllMembershipPurposes;
using DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Command;
using DHAFacilitationAPIs.Application.Feature.Religion.Queries;
using DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;
using DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
using DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "club")]
public class MembershipPurposesController : BaseApiController
{
    [HttpGet("GetMembershipPurposes"), AllowAnonymous]
    public async Task<IActionResult> GetMembershipPurposes()
    {
        return Ok(await Mediator.Send(new GetAllMembershipPurposesQuery()));
    }

    [HttpPost("MemberShip-Request")]
    public async Task<IActionResult> AddMembershipPurposes([FromForm] CreateMemberShipRequestCommand request,CancellationToken ct)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpGet("get-MemberShips"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShips(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAllMemberShipQuery(), ct);
        return Ok(result);
    }
    [HttpGet("get-MemberShipCategories"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShipsCategories(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAllMemberShipCategoryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-MemberShipCategoriesById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllMemberShipsCategoriesById(Guid MemberShipId, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetMemberShipCategoryByIdQuery(MemberShipId), ct);
        return Ok(result);
    }

    [HttpGet("get-Religons"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligon(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAllReligonCategoryQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-ReligonSect-ById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSectById(Guid ReligonId, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetReligonSectByIdQuery(ReligonId), ct);
        return Ok(result);
    }


    //[HttpPost("AddMembershipPurposes")]

    //public async Task<IActionResult> AddMembershipPurposes(AddMembershipPurposeCommand request)
    //{
    //    return Ok(await Mediator.Send(request));
    //}
}

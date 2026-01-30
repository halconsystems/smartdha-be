using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries.GetClubCategories;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetComplaintDropdowns;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaints;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "club")]
public class ClubController : BaseApiController
{
    private readonly IMediator _mediator;
    public ClubController(IMediator med) => _mediator = med;

    [HttpGet("Get-AllClubs-List")]
    public async Task<IActionResult> Types(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAllClubQuery(),ct);
        return Ok(result);
    }

    // 2️⃣ Get club detail by id (Detail screen)
    [HttpGet("{clubId:guid}")]
    public async Task<IActionResult> GetClubById(Guid clubId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetClubDetailById(clubId),
            ct
        );

        return Ok(result);
    }

    // 3️⃣ Get categories of a club (Tabs)
    [HttpGet("{clubId:guid}/categories")]
    public async Task<IActionResult> GetClubCategories(
        Guid clubId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetClubCategoriesQuery(clubId),
            ct
        );

        return Ok(result);
    }

    // 4️⃣ Get facilities by category (Tab click)
    [HttpGet("{clubId:guid}/categories/{categoryId:guid}/facilities")]
    public async Task<IActionResult> GetFacilitiesByCategory(
        Guid clubId,
        Guid categoryId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetClubFacilitiesByCategoryQuery(clubId, categoryId),
            ct
        );

        return Ok(result);
    }

    //[HttpGet("Get-ClubHistory")]
    //public async Task<IActionResult> Types(Guid Id,CancellationToken ct)
    //{
    //    var result = await Mediator.Send(new GetClubDetailById(Id), ct);
    //    return Ok(result);
    //}

    //[HttpGet("by-category/{categoryId:guid}")]
    //public async Task<IActionResult> GetByCategory(
    //    Guid categoryId,
    //    CancellationToken ct)
    //{
    //    return Ok(await Mediator.Send(
    //        new GetClubServiceProcessByCatQuery(categoryId), ct));
    //}

    //[HttpGet("Club-service{processId:guid}")]
    //public async Task<IActionResult> GetServiceById(
    //  Guid processId,
    //  CancellationToken ct)
    //{
    //    return Ok(await Mediator.Send(
    //        new GetClubServiceProvessByIdQuery(processId), ct));
    //}
}

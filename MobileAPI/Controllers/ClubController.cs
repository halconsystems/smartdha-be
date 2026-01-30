using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetComplaintDropdowns;
using DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaints;
using Microsoft.AspNetCore.Mvc;
using MobileAPI.Authorization;

namespace MobileAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "other")]
public class ClubController : BaseApiController
{
    private readonly IMediator _med;
    public ClubController(IMediator med) => _med = med;

    [HttpGet("Get-AllClubs-List")]
    public async Task<IActionResult> Types(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAllClubQuery(),ct);
        return Ok(result);
    }

    [HttpGet("Get-ClubHistory")]
    public async Task<IActionResult> Types(Guid Id,CancellationToken ct)
    {
        var result = await Mediator.Send(new GetClubDetailById(Id), ct);
        return Ok(result);
    }

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategory(
        Guid categoryId,
        CancellationToken ct)
    {
        return Ok(await Mediator.Send(
            new GetClubServiceProcessByCatQuery(categoryId), ct));
    }

    [HttpGet("Club-service{processId:guid}")]
    public async Task<IActionResult> GetServiceById(
      Guid processId,
      CancellationToken ct)
    {
        return Ok(await Mediator.Send(
            new GetClubServiceProvessByIdQuery(processId), ct));
    }
}

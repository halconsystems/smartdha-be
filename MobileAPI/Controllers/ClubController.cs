using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Security;
using DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Commands.CreateBooking;
using DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Queries.GetBookingDetail;
using DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Queries.MyBookings;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries.GetClubCategories;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityAvailability.Queries.SearchFacilityAvailability;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries.FacilityUnitDetail;
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
        var result = await _mediator.Send(new GetClubFacilitiesByCategoryQuery(clubId, categoryId), ct);
        return Ok(result);
    }

    [HttpGet("{clubId}/facility-search")]
    public Task<ApiResult<List<FacilitySearchResponse>>> Search(
        Guid clubId,
        [FromQuery] Guid? facilityId,
        [FromQuery] DateOnly? date,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        return _mediator.Send(new SearchFacilityAvailabilityQuery(
            clubId,
            facilityId,
            date,
            fromDate,
            toDate
        ));
    }
    [HttpGet("{facilityUnitId}/GetFacilityUnitDetail")]
   
    public Task<ApiResult<FacilityUnitDetailResponse>> GetDetail(
        Guid facilityUnitId)
        => _mediator.Send(new GetFacilityUnitDetailQuery(facilityUnitId));

    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingRequest request,
        CancellationToken ct)
    {
        var command = new CreateBookingCommand(
            request.ClubId,
            request.FacilityId,
            request.FacilityUnitId,

            request.BookingMode,

            request.SlotRequest,
            request.DateRangeRequest,

            request.DiscountPercent
        );
        var result = await _mediator.Send(command, ct);
        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet("GetMyBookings")]
    public async Task<IActionResult> GetMyBookings(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyBookingsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{bookingId:guid}/GetBookings")]
    public async Task<IActionResult> GetBookingDetail(
    Guid bookingId,
    CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetBookingDetailQuery(bookingId), ct);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }

}

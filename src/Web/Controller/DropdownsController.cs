using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/dropdowns")]
public class DropdownsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public DropdownsController(IMediator mediator, IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet("clubs")]
    public async Task<ActionResult<List<DropdownDto>>> Clubs([FromQuery] ClubType? clubType, CancellationToken ct)
    {
        // 1️⃣ Get all clubs (generic handler)
        var allClubs = await _mediator.Send(new GetDropdownQuery<Club>(ClubType: clubType), ct);

        // 2️⃣ Get current user
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 3️⃣ Get current roles
        var roles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        if (isSuperAdmin)
        {
            // ✅ SuperAdmin → return all clubs
            return Ok(allClubs);
        }

        // 4️⃣ Normal user → filter by assigned clubs
        var assignedClubIds = await _context.UserClubAssignments
            .Where(uca => uca.UserId == userId)
            .Select(uca => uca.ClubId)
            .ToListAsync(ct);

        var filteredClubs = allClubs
            .Where(c => assignedClubIds.Contains(c.Id))
            .OrderBy(c => c.Name)
            .ToList();

        return Ok(filteredClubs);
    }

    [HttpGet("residence-types")]
    public async Task<ActionResult<List<DropdownDto>>> GetResidenceTypes([FromQuery] ClubType? clubType, CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<ResidenceType>(ClubType: clubType), ct);
        return Ok(data);
    }

    [HttpGet("room-categories")]
    public async Task<ActionResult<List<DropdownDto>>> GetRoomCategories([FromQuery] ClubType? clubType, CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<RoomCategory>(ClubType: clubType), ct);
        return Ok(data);
    }

    [HttpGet("services")]
    public async Task<ActionResult<List<DropdownDto>>> GetServices([FromQuery] ServiceType? serviceType, CancellationToken ct)
    {
        var data = await _mediator.Send(new GetDropdownQuery<DHAFacilitationAPIs.Domain.Entities.Services>(ServiceType: serviceType), ct);
        return Ok(data);
    }
}


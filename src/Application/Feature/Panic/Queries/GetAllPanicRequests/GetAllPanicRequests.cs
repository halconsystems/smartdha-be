using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicRequests;
public record GetAllPanicRequestsQuery() : IRequest<List<PanicCreatedRealtimeDto>>;

public class GetAllPanicRequestsQueryHandler
    : IRequestHandler<GetAllPanicRequestsQuery, List<PanicCreatedRealtimeDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllPanicRequestsQueryHandler(IApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
    }

    public async Task<List<PanicCreatedRealtimeDto>> Handle(GetAllPanicRequestsQuery r, CancellationToken ct)
    {
        // 1. Load panic requests only
        var requests = await _ctx.PanicRequests
            .AsNoTracking()
            .Include(x => x.EmergencyType)
            .OrderByDescending(x => x.Created)
            .ToListAsync(ct);

        // 2. Collect distinct UserIds
        var userIds = requests.Select(x => x.RequestedByUserId).Distinct().ToList();

        // 3. Load users from Identity
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(ct);

        // 4. Create lookup dictionary
        var userLookup = users.ToDictionary(u => u.Id);

        // 5. Map into DTO
        var dtos = requests.Select(x =>
        {
            userLookup.TryGetValue(x.RequestedByUserId, out var user);

            return new PanicCreatedRealtimeDto(
                x.Id,
                x.CaseNo,
                x.EmergencyType.Code,
                x.EmergencyType.Name,
                x.Latitude,
                x.Longitude,
                (PanicStatus)x.Status,
                x.Created,

                user?.Name ?? "",   // 👈 adjust property names
                user?.Email ?? "",
                user?.PhoneNumber ?? "",
                (UserType)(user?.UserType ?? UserType.NonMember) // 👈 fallback if null
            );
        }).ToList();

        return dtos;
    }
}




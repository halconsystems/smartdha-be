using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
public record GetPanicByIdQuery(Guid Id) : IRequest<PanicDetailDto>;

public class GetPanicByIdQueryHandler : IRequestHandler<GetPanicByIdQuery, PanicDetailDto>
{
    private readonly IApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetPanicByIdQueryHandler(IApplicationDbContext ctx, UserManager<ApplicationUser> userManager) => (_ctx, _userManager) = (ctx, userManager);

    public async Task<PanicDetailDto> Handle(GetPanicByIdQuery r, CancellationToken ct)
    {
        var e = await _ctx.PanicRequests.AsNoTracking()
            .Include(x => x.EmergencyType)
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct)
            ?? throw new KeyNotFoundException("Panic not found.");

        var RequestedByUser = await _userManager.Users
            .Where(u => u.Id == e.RequestedByUserId)
            .FirstOrDefaultAsync(ct);

        if (RequestedByUser == null)
            throw new KeyNotFoundException("RequestedByUser not found.");

        var AssignedToUser = await _ctx.PanicResponders
            .Where(u => u.Id.ToString() == e.AssignedToUserId)
            .FirstOrDefaultAsync(ct);

        return new PanicDetailDto(
            e.Id, e.CaseNo, e.EmergencyType.Code, e.EmergencyType.Name,
            e.Latitude, e.Longitude, (PanicStatus)e.Status,
            e.Created, e.AcknowledgedAt, e.ResolvedAt, e.CancelledAt,
            e.RequestedByUserId, RequestedByUser.Name, RequestedByUser.Email, RequestedByUser.MobileNo, RequestedByUser.UserType,
            e.AssignedToUserId, AssignedToUser?.Name, AssignedToUser?.Email, AssignedToUser?.PhoneNumber, e.Notes, e.MediaUrl
        );
    }
}

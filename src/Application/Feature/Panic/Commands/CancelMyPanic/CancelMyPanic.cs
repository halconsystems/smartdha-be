using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using static Dapper.SqlMapper;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Featurentity.Panic.Commands.CancelMyPanic;
public record CancelMyPanicCommand(Guid PanicRequestId, string? Remarks) : IRequest<Unit>;

public class CancelMyPanicCommandHandler
    : IRequestHandler<CancelMyPanicCommand, Unit>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly IPanicRealtime _realtime;
    private readonly UserManager<ApplicationUser> _userManager;

    public CancelMyPanicCommandHandler(
        IApplicationDbContext ctx,
        ICurrentUserService current,
        IPanicRealtime realtime,UserManager<ApplicationUser> userManager)
        => (_ctx, _current, _realtime, _userManager) = (ctx, current, realtime, userManager);

    public async Task<Unit> Handle(CancelMyPanicCommand r, CancellationToken ct)
    {
        var uid = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == uid, ct)
            ?? throw new NotFoundException("User not found.");

        var entity = await _ctx.PanicRequests
            .FirstOrDefaultAsync(x => x.Id == r.PanicRequestId && x.RequestedByUserId == uid, ct)
            ?? throw new NotFoundException("Panic request not found.");

        if (entity.Status is PanicStatus.Resolved or PanicStatus.Cancelled)
            return Unit.Value;

        var from = entity.Status;
        entity.Status = PanicStatus.Cancelled;
        entity.CancelledAt = DateTime.Now;

        _ctx.PanicActionLogs.Add(new PanicActionLog
        {
            PanicRequestId = entity.Id,
            ActionByUserId = uid,
            Action = "CANCEL",
            Remarks = r.Remarks,
            FromStatus = from,
            ToStatus = entity.Status
        });

        // 1️⃣ Find last dispatch
        var lastDispatch = await _ctx.PanicDispatches
            .Where(d => d.PanicRequestId == entity.Id)
            .OrderByDescending(d => d.AssignedAt)
            .FirstOrDefaultAsync(ct);

        if (lastDispatch != null)
        {
            // 2️⃣ Close dispatch
            lastDispatch.Status = PanicDispatchStatus.Cancelled;
            lastDispatch.CancelledAt = DateTime.Now;

            // 3️⃣ Release vehicle
            var veh = await _ctx.SvVehicles
                .FirstOrDefaultAsync(v => v.Id == lastDispatch.SvVehicleId, ct);

            if (veh != null)
            {
                veh.Status = SvVehicleStatus.Available;
            }
        }

        await _ctx.SaveChangesAsync(ct);

        var et = await _ctx.EmergencyTypes
            .FirstOrDefaultAsync(x => x.Id == entity.EmergencyTypeId, ct)
            ?? throw new NotFoundException("Emergency type not found.");

        var rtDto = new PanicCreatedRealtimeDto(
           Id: entity.Id,
           CaseNo: entity.CaseNo,
           EmergencyCode: et.Code,
           EmergencyName: et.Name,
           Latitude: entity.Latitude,
           Longitude: entity.Longitude,
           Status: entity.Status,
           CreatedUtc: entity.Created,
           Address: entity.Address ?? "",
           Note: entity.Notes ?? "",
           MobileNumber: entity.MobileNumber ?? "",

           RequestedByName: user.Name,
           RequestedByEmail: user.Email ?? user.RegisteredEmail ?? string.Empty,
           RequestedByPhone: user.MobileNo ?? user.RegisteredMobileNo ?? string.Empty,
           RequestedByUserType: user.UserType
       );

        //await _realtimentity.PanicCreatedAsync(rtDto);
        try
        {
            await _realtime.PanicCreatedAsync(rtDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            //_logger.LogWarning(ex, "PanicCreated realtime broadcast failed for {CaseNo}", entity.CaseNo);
            // don't rethrow; creation succeeded already
        }

        return Unit.Value;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyActivePanic;
public record GetMyActivePanicQuery : IRequest<List<PanicRequestsDto>>;

public class GetMyActivePanicQueryHandler : IRequestHandler<GetMyActivePanicQuery, List<PanicRequestsDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVehicleLocationStore _fileService;

    public GetMyActivePanicQueryHandler(IApplicationDbContext ctx, ICurrentUserService current, UserManager<ApplicationUser> userManager,IVehicleLocationStore fileService)
        => (_ctx, _current,_userManager,_fileService) = (ctx, current,userManager,fileService);

    public async Task<List<PanicRequestsDto>> Handle(GetMyActivePanicQuery r, CancellationToken ct)
    {
        var uid = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in.");

        var data = await _ctx.PanicRequests
         .AsNoTracking()
         .Include(x => x.EmergencyType)
         .Include(x => x.PanicDispatches)
             .ThenInclude(d => d.SvVehicle)
        .Where(x =>
    x.RequestedByUserId == uid &&
    (
        (x.Status != PanicStatus.Resolved && x.Status != PanicStatus.Cancelled)
        ||
        (x.Status == PanicStatus.Resolved && x.TakeReview == true)
    )
)
         .OrderByDescending(x => x.Created)
         .ToListAsync(ct);

        var driverIds = data
        .SelectMany(p => p.PanicDispatches)
        .Select(d => d.SvVehicle?.DriverUserId)
        .Where(id => !string.IsNullOrWhiteSpace(id))
        .Distinct()
        .ToList();

        var drivers = await _userManager.Users
        .AsNoTracking()
        .Where(u => driverIds.Contains(u.Id))
        .ToListAsync(ct);

        var driverLookup = drivers.ToDictionary(d => d.Id);

        var result = new List<PanicRequestsDto>();

        foreach (var x in data)
        {
            var lastDispatch = x.PanicDispatches
                .OrderByDescending(d => d.AssignedAt)
                .FirstOrDefault();

            var v = lastDispatch?.SvVehicle;

            // DRIVER
            ApplicationUser? driver = null;
            if (v?.DriverUserId != null)
                driverLookup.TryGetValue(v.DriverUserId, out driver);

            // ✅ SINGLE vehicle → SINGLE location lookup
            VehicleLocationDto? location = null;
            if (v != null)
                location = await _fileService.GetLocationAsync(v.Id);

            // JSON overrides DB
            double? lat = location?.Latitude ?? v?.LastLatitude;
            double? lng = location?.Longitude ?? v?.LastLongitude;
            DateTime? lastAt = location?.LastLocationUpdateAt ?? v?.LastLocationAt;

            result.Add(new PanicRequestsDto(
                x.Id,
                x.CaseNo,
                x.EmergencyType.Code,
                x.EmergencyType.Name,
                x.Latitude,
                x.Longitude,
                x.Status,
                x.Created,
                x.TakeReview,

                // VEHICLE
                v?.Id,
                v?.Name,
                v?.RegistrationNo,
                v?.VehicleType.ToString(),
                lastDispatch?.Status.ToString(),
                lat,
                lng,
                lastAt,

                // DRIVER
                driver?.Id ?? "",
                driver?.Name ?? "",
                driver?.Email,
                driver?.MobileNo ?? driver?.RegisteredMobileNo,
                driver?.CNIC
            ));
        }




        //var result = data.Select(x =>
        //{
        //    // Filter dispatch for THIS panic only (x.Id)
        //    var lastDispatch = x.PanicDispatches
        //        .Where(d => d.PanicRequestId == x.Id)   // FIXED
        //        .OrderByDescending(d => d.AssignedAt)
        //        .FirstOrDefault();

        //    var v = lastDispatch?.SvVehicle;

        //    ApplicationUser? driver = null;
        //    if (v?.DriverUserId != null)
        //        driverLookup.TryGetValue(v.DriverUserId, out driver);


        //    VehicleLocationDto? location = null;
        //    if (v != null)
        //        location = await _fileService.GetLocationAsync(v.Id);

        //    return new PanicRequestsDto(
        //        x.Id,
        //        x.CaseNo,
        //        x.EmergencyType.Code,
        //        x.EmergencyType.Name,
        //        x.Latitude,
        //        x.Longitude,
        //        x.Status,
        //        x.Created,

        //        // VEHICLE FIELDS
        //        v?.Id,
        //        v?.Name,
        //        v?.RegistrationNo,
        //        v?.VehicleType.ToString(),
        //        lastDispatch?.Status.ToString(),
        //        v?.LastLatitude,
        //        v?.LastLongitude,
        //        v?.LastLocationAt,

        //        // DRIVER INFO
        //        driver?.Id ?? "",
        //        driver?.Name ?? "",
        //        driver?.Email,
        //        driver?.MobileNo ?? driver?.RegisteredMobileNo,
        //        driver?.CNIC
        //    );
        //})
        //.ToList();

        return result;

    }
}

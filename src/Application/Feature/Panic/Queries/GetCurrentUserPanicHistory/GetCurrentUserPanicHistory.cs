using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetCurrentUserPanicHistory;
public record GetCurrentUserPanicHistoryQuery : IRequest<List<PanicHistoryDetailDto>>;

public class GetCurrentUserPanicHistoryQueryHandler
    : IRequestHandler<GetCurrentUserPanicHistoryQuery, List<PanicHistoryDetailDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVehicleLocationStore _vehicleLocationStore;
    private readonly IFileStorageService _fileStorageService;

    public GetCurrentUserPanicHistoryQueryHandler(
        IApplicationDbContext ctx,
        ICurrentUserService current,
        UserManager<ApplicationUser> userManager,
        IVehicleLocationStore vehicleLocationStore,
        IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _current = current;
        _userManager = userManager;
        _vehicleLocationStore = vehicleLocationStore;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<PanicHistoryDetailDto>> Handle(GetCurrentUserPanicHistoryQuery r, CancellationToken ct)
    {
        var uid = _current.UserId.ToString()
            ?? throw new UnAuthorizedException("Not signed in.");

        // Load ALL completed/cancelled panics of user
        var panics = await _ctx.PanicRequests
            .AsNoTracking()
            .Include(p => p.EmergencyType)
            .Where(p =>
                p.RequestedByUserId == uid)
            .OrderByDescending(p => p.Created)
            .ToListAsync(ct);

        var result = new List<PanicHistoryDetailDto>();

        foreach (var panic in panics)
        {
            // Latest dispatch
            var dispatch = await _ctx.PanicDispatches
                .Include(d => d.SvVehicle)
                .Where(d => d.PanicRequestId == panic.Id)
                .OrderByDescending(d => d.AssignedAt)
                .FirstOrDefaultAsync(ct);

            if (dispatch == null)
                continue;

            var vehicle = dispatch.SvVehicle;

            // Driver resolution (same logic you used)
            ApplicationUser? driver = null;

            if (!string.IsNullOrWhiteSpace(dispatch.DriverUserId))
            {
                driver = await _userManager.FindByIdAsync(dispatch.DriverUserId);
            }
            else if (!string.IsNullOrWhiteSpace(dispatch.LastModifiedBy))
            {
                driver = await _userManager.FindByIdAsync(dispatch.LastModifiedBy);
            }
            else if (!string.IsNullOrWhiteSpace(dispatch.CreatedBy))
            {
                driver = await _userManager.FindByIdAsync(dispatch.CreatedBy);
            }

            // Live location
            var jsonLoc = await _vehicleLocationStore.GetLocationAsync(vehicle.Id);

            var finalLat = jsonLoc?.Latitude ?? vehicle.LastLatitude;
            var finalLng = jsonLoc?.Longitude ?? vehicle.LastLongitude;
            var finalTime = jsonLoc?.LastLocationUpdateAt ?? vehicle.LastLocationAt;

            // Completion media
            var media = await _ctx.PanicDispatchMedias
                .Where(m => m.PanicDispatchId == dispatch.Id)
                .OrderBy(m => m.Created)
                .ToListAsync(ct);

            // Review (optional)
            var review = await _ctx.PanicReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(rw => rw.PanicRequestId == panic.Id, ct);

            result.Add(new PanicHistoryDetailDto
            {
                PanicId = panic.Id,
                CaseNo = panic.CaseNo,
                PanicLatitude = panic.Latitude,
                PanicLongitude = panic.Longitude,
                PanicStatus = panic.Status,
                CreatedAt = panic.Created,
                Address = panic.Address ?? "",
                Note = panic.Notes ?? "",

                DriverRemarks = dispatch.DriverRemarks ?? "",
                ControlRoomRemarks = dispatch.ControlRoomRemarks ?? "",
                FinalAudioNote = string.IsNullOrWhiteSpace(dispatch.DriverRemarksAudioPath)
                    ? string.Empty
                    : _fileStorageService.GetPublicUrl(dispatch.DriverRemarksAudioPath),

                EmergencyName = panic.EmergencyType.Name,

                DispatchId = dispatch.Id,
                AssignedAt = dispatch.AssignedAt,
                AcceptedAt = dispatch.AcceptedAt,
                ArrivedAt = dispatch.ArrivedAt,
                CompletedAt = dispatch.CompletedAt,

                // Vehicle
                VehicleId = vehicle.Id,
                VehicleName = vehicle.Name,
                RegistrationNo = vehicle.RegistrationNo,
                VehicleType = vehicle.VehicleType.ToString(),
                VehicleStatus = vehicle.Status.ToString(),
                MapIconKey = vehicle.MapIconKey,

                LastLatitude = finalLat,
                LastLongitude = finalLng,
                LastLocationAt = finalTime,

                // Driver
                DriverUserId = driver?.Id ?? "",
                DriverName = driver?.Name ?? "",
                DriverEmail = driver?.Email,
                DriverPhone = driver?.MobileNo ?? driver?.RegisteredMobileNo,
                DriverCnic = driver?.CNIC,

                // Review info
                Review = review == null
                    ? null
                    : new PanicReviewDto
                    {
                        Rating = review.Rating,
                        ReviewText = review.ReviewText,
                        CreatedAt = review.Created
                    },

                // Media
                CompletionMedia = media.Select(m => new PanicDispatchMediaDto
                {
                    MediaType = m.MediaType,
                    Url = _fileStorageService.GetPublicUrl(m.FilePath)
                }).ToList(),

                // Geo metrics
                AcceptedAtLatitude = dispatch.AcceptedAtLatitude,
                AcceptedAtLongitude = dispatch.AcceptedAtLongitude,
                AcceptedAtAddress = dispatch.AcceptedAtAddress,
                DistanceFromPanicKm = dispatch.DistanceFromPanicKm,
                LastLocationUpdateAt = dispatch.LastLocationUpdateAt
            });
        }

        return result;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
public record GetPanicHistoryByIdQuery(Guid Id) : IRequest<PanicHistoryDetailDto>;

public class GetPanicHistoryByIdQueryHandler
    : IRequestHandler<GetPanicHistoryByIdQuery, PanicHistoryDetailDto>
{
    private readonly IApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVehicleLocationStore _vehicleLocationStore;
    private readonly IFileStorageService _fileStorageService;

    public GetPanicHistoryByIdQueryHandler(
        IApplicationDbContext ctx,
        UserManager<ApplicationUser> userManager,
        IVehicleLocationStore vehicleLocationStore,
        IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _userManager = userManager;
        _vehicleLocationStore = vehicleLocationStore;
        _fileStorageService = fileStorageService;
    }

    public async Task<PanicHistoryDetailDto> Handle(GetPanicHistoryByIdQuery r, CancellationToken ct)
    {
        // Load the panic
        var panic = await _ctx.PanicRequests
            .Include(p => p.EmergencyType)
            .FirstOrDefaultAsync(p => p.Id == r.Id, ct)
            ?? throw new NotFoundException("Panic not found.");

        // Load requester user
        var requester = await _userManager.FindByIdAsync(panic.RequestedByUserId.ToString())
            ?? throw new NotFoundException("Requester user not found.");

        // Load dispatch (latest for that panic)
        var dispatch = await _ctx.PanicDispatches
            .Include(d => d.SvVehicle)
            .Where(x => x.PanicRequestId == panic.Id)
            .OrderByDescending(x => x.AssignedAt)
            .FirstOrDefaultAsync(ct);

        if (dispatch == null)
            throw new NotFoundException("No dispatch found for this panic.");

        var vehicle = dispatch.SvVehicle;

        // Load driver
        //var driver = await _userManager.FindByIdAsync(vehicle.DriverUserId!.ToString());

        ApplicationUser? driver = null;

        // Priority 1: use LastModifiedBy if exists
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

        // Live JSON Location
        var jsonLoc = await _vehicleLocationStore.GetLocationAsync(vehicle.Id);

        double? finalLat = jsonLoc?.Latitude ?? vehicle.LastLatitude;
        double? finalLng = jsonLoc?.Longitude ?? vehicle.LastLongitude;
        DateTime? finalTime = jsonLoc?.LastLocationUpdateAt ?? vehicle.LastLocationAt;

        var media = await _ctx.PanicDispatchMedias
            .Where(m => m.PanicDispatchId == dispatch.Id && dispatch.IsActive==true)
            .OrderBy(m => m.Created)
            .ToListAsync(ct);

        // Review (optional)
        var review = await _ctx.PanicReviews
            .AsNoTracking()
            .FirstOrDefaultAsync(rw => rw.PanicRequestId == panic.Id, ct);

        // Construct DTO
        return new PanicHistoryDetailDto
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
            FinalAudioNote = string.IsNullOrWhiteSpace(dispatch.DriverRemarksAudioPath) ? string.Empty : _fileStorageService.GetPublicUrl(dispatch.DriverRemarksAudioPath),
            EmergencyName = panic.EmergencyType.Name,

            RequestedByUserId = panic.RequestedByUserId.ToString(),
            RequestedByName = requester.Name,
            RequestedByEmail = requester.Email,
            RequestedByPhone =
                requester.MobileNo
                ?? requester.RegisteredMobileNo
                ?? panic.MobileNumber,
            RequestedByUserType = requester.UserType,

            DispatchId = dispatch.Id,

            AssignedAt = dispatch.AssignedAt,
            AcceptedAt = dispatch.AcceptedAt,
            ArrivedAt = dispatch.ArrivedAt,
            CompletedAt = dispatch.CompletedAt,

            // ——— Timeline Minutes ———
            // Created → AssignedAt
            CreatedToAssignedMinutes =
    (dispatch.AssignedAt != default)
        ? (dispatch.AssignedAt - panic.Created).TotalMinutes
        : (double?)null,

            // AssignedAt → AcceptedAt
            AssignedToAcceptedMinutes =
    dispatch.AcceptedAt.HasValue
        ? (dispatch.AcceptedAt.Value - dispatch.AssignedAt).TotalMinutes
        : (double?)null,

            // AcceptedAt → ArrivedAt
            AcceptedToArrivedMinutes =
    (dispatch.AcceptedAt.HasValue && dispatch.ArrivedAt.HasValue)
        ? (dispatch.ArrivedAt.Value - dispatch.AcceptedAt.Value).TotalMinutes
        : (double?)null,

            // ArrivedAt → CompletedAt
            ArrivedToCompletedMinutes =
    (dispatch.ArrivedAt.HasValue && dispatch.CompletedAt.HasValue)
        ? (dispatch.CompletedAt.Value - dispatch.ArrivedAt.Value).TotalMinutes
        : (double?)null,

            // Created → CompletedAt (TOTAL)
            TotalCompletionMinutes =
    dispatch.CompletedAt.HasValue
        ? (dispatch.CompletedAt.Value - panic.Created).TotalMinutes
        : (double?)null,


            // DRIVER INFO
            DriverUserId = driver?.Id ?? "",
            DriverName = driver?.Name ?? "",
            DriverEmail = driver?.Email,
            DriverPhone = driver?.MobileNo ?? driver?.RegisteredMobileNo,
            DriverCnic = driver?.CNIC,

            // VEHICLE INFO
            VehicleId = vehicle.Id,
            VehicleName = vehicle.Name,
            RegistrationNo = vehicle.RegistrationNo,
            VehicleType = vehicle.VehicleType.ToString(),
            VehicleStatus = vehicle.Status.ToString(),
            MapIconKey = vehicle.MapIconKey,

            LastLatitude = finalLat,
            LastLongitude = finalLng,
            LastLocationAt = finalTime,
            // ✅ ADD THIS
            CompletionMedia = media.Select(m => new PanicDispatchMediaDto
            {
                MediaType = m.MediaType,
                Url = _fileStorageService.GetPublicUrl(m.FilePath)
            }).ToList(),
            AcceptedAtLatitude=dispatch.AcceptedAtLatitude,
            AcceptedAtLongitude=dispatch.AcceptedAtLongitude,
            AcceptedAtAddress=dispatch.AcceptedAtAddress,
            DistanceFromPanicKm= dispatch.DistanceFromPanicKm,
            LastLocationUpdateAt= dispatch.LastLocationUpdateAt,
            // Review info
            Review = review == null
                    ? null
                    : new PanicReviewDto
                    {
                        Rating = review.Rating,
                        ReviewText = review.ReviewText,
                        CreatedAt = review.Created
                    },
        };
    }
}


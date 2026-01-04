using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CompletePanicDispatch;
public class CompletePanicDispatchCommand : IRequest<string>
{
    public Guid DispatchId { get; set; }
    public string? DriverRemarks { get; set; }
    public IFormFile? VoiceFile { get; set; }   // optional mp3
    public List<IFormFile>? MediaFiles { get; set; }  // optional images / videos
}

public class CompletePanicDispatchCommandHandler
    : IRequestHandler<CompletePanicDispatchCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPanicRealtime _realtime;
    private readonly IFileStorageService _fileStorage;

    public CompletePanicDispatchCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager,
        IPanicRealtime realtime,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
        _realtime = realtime;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(CompletePanicDispatchCommand request, CancellationToken ct)
    {
        var driverId = _currentUser.UserId.ToString();

        if (driverId == null)
            throw new UnauthorizedAccessException("Driver not authenticated");

        var driver = await _userManager.FindByIdAsync(driverId);
        if (driver == null)
            throw new UnauthorizedAccessException("Driver not found");

        // Load dispatch
        var dispatch = await _context.PanicDispatches
            .Include(x => x.PanicRequest)
            .Include(x => x.SvVehicle)
            .FirstOrDefaultAsync(x => x.Id == request.DispatchId, ct)
            ?? throw new NotFoundException("Dispatch not found");

        var getPanic = await _context.PanicRequests
            .FirstOrDefaultAsync(x => x.Id == dispatch.PanicRequestId, ct)
            ?? throw new NotFoundException("Panic not found");
        getPanic.ResolvedAt= DateTime.Now;

        // Ensure this is the assigned driver
        if (dispatch.SvVehicle.DriverUserId != driverId)
            throw new UnauthorizedAccessException("You cannot complete a dispatch not assigned to you.");

        // Validate dispatch status
        if (dispatch.Status is PanicDispatchStatus.Completed or PanicDispatchStatus.Cancelled)
            throw new InvalidOperationException("This dispatch is already completed or cancelled.");

        // ------------------------------
        // Update dispatch
        // ------------------------------
        dispatch.Status = PanicDispatchStatus.Completed;
        dispatch.CompletedAt = DateTime.Now;
        dispatch.DriverRemarks = request.DriverRemarks;

        // ✅ Optional voice (.mp3 only)
        if (request.VoiceFile != null)
        {
            var allowedExtensions = new[] { ".mp3", ".aac" };

            var relativePath = await _fileStorage.SaveAudioAsync(
                request.VoiceFile,
                folderName: "panic-driver-remarks",
                ct,
                maxBytes: 10 * 1024 * 1024,
                allowedExtensions: allowedExtensions
            );

            dispatch.DriverRemarksAudioPath = relativePath;
        }

        // ------------------------------
        // Save completion images / videos (optional)
        // ------------------------------
        if (request.MediaFiles != null && request.MediaFiles.Any())
        {
            foreach (var file in request.MediaFiles)
            {
                var result = await _fileStorage.SaveImageOrVideoAsync(
                    file,
                    folderName: "panic-dispatch-completion",
                    ct
                );

                _context.PanicDispatchMedias.Add(new PanicDispatchMedia
                {
                    PanicDispatchId = dispatch.Id,
                    FilePath        = result.Path,
                    MediaType       = result.MediaType
                });
            }
        }
        // ------------------------------
        // Update panic
        // ------------------------------
        dispatch.PanicRequest.Status = PanicStatus.Resolved;
        dispatch.PanicRequest.TakeReview = true;
        // ------------------------------
        // Update vehicle
        // ------------------------------
        dispatch.SvVehicle.Status = SvVehicleStatus.Available;

        await _context.SaveChangesAsync(ct);

        // ------------------------------
        // Send Realtime Update
        // ------------------------------
        var dto = new PanicUpdatedRealtimeDto
        {
            PanicId = dispatch.PanicRequestId,
            DispatchId = dispatch.Id,

            PanicStatus = dispatch.PanicRequest.Status,

            AssignedAt = dispatch.AssignedAt,
            AcceptedAt = dispatch.AcceptedAt,
            ArrivedAt = dispatch.ArrivedAt,
            CompletedAt = dispatch.CompletedAt,

            VehicleId = dispatch.SvVehicle.Id,
            VehicleName = dispatch.SvVehicle.Name,
            RegistrationNo = dispatch.SvVehicle.RegistrationNo,
            VehicleType = dispatch.SvVehicle.VehicleType.ToString(),
            VehicleStatus = dispatch.SvVehicle.Status.ToString(),
            MapIconKey = dispatch.SvVehicle.MapIconKey,

            LastLatitude = dispatch.SvVehicle.LastLatitude,
            LastLongitude = dispatch.SvVehicle.LastLongitude,
            LastLocationAt = dispatch.SvVehicle.LastLocationAt,

            RequestedByName = driver.Name,
            RequestedByEmail = driver.Email ?? "",
            RequestedByPhone = driver.PhoneNumber ?? "",
        };

        await _realtime.SendPanicUpdatedAsync(dto, ct);

        return "Panic task completed successfully.";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.ReachedPanicDispatch;
public class ReachedPanicDispatchCommand : IRequest<string>
{
    public Guid DispatchId { get; set; }
}

public class ReachedPanicDispatchCommandHandler
    : IRequestHandler<ReachedPanicDispatchCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPanicRealtime _realtime;

    public ReachedPanicDispatchCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager,
        IPanicRealtime realtime)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
        _realtime = realtime;
    }

    public async Task<string> Handle(ReachedPanicDispatchCommand request, CancellationToken ct)
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

        // Ensure this is the assigned driver
        if (dispatch.SvVehicle.DriverUserId != driverId)
            throw new UnauthorizedAccessException("You cannot complete a dispatch not assigned to you.");

        // Validate dispatch status
        if (dispatch.Status is PanicDispatchStatus.Completed or PanicDispatchStatus.Cancelled)
            throw new InvalidOperationException("This dispatch is already completed or cancelled.");

        // ------------------------------
        // Update dispatch
        // ------------------------------
        dispatch.Status = PanicDispatchStatus.Arrived;
        dispatch.ArrivedAt = DateTime.Now;

        // ------------------------------
        // Update panic
        // ------------------------------
        dispatch.PanicRequest.Status = PanicStatus.Arrived;

        // ------------------------------
        // Update vehicle
        // ------------------------------
        dispatch.SvVehicle.Status = SvVehicleStatus.Busy;

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

            RequestedByName = driver.Name,
            RequestedByEmail = driver.Email ?? "",
            RequestedByPhone = driver.PhoneNumber ?? "",
        };

        await _realtime.SendPanicUpdatedAsync(dto, ct);
        return "Vehicle has reached the panic location.";
    }
}


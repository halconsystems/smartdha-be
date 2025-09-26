using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.UpdateDriverAssignment;
public record UpdateDriverAssignmentCommand(UpdateDriverAssignmentDto Dto) : IRequest<SuccessResponse<Guid>>;

public class UpdateDriverAssignmentCommandHandler : IRequestHandler<UpdateDriverAssignmentCommand, SuccessResponse<Guid>>
{
    private readonly IOLHApplicationDbContext _context;

    public UpdateDriverAssignmentCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateDriverAssignmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var bowserRequest = await _context.BowserRequests
            .FirstOrDefaultAsync(br => br.Id == dto.BowserRequestId, cancellationToken);

        if (bowserRequest == null)
            throw new NotFoundException(nameof(OLH_BowserRequest), dto.BowserRequestId.ToString());

        string message = "";

        // Case 1: Driver & Vehicle update/unassign
        if (dto.DriverId.HasValue)
        {
            // Verify Driver exists
            var driver = await _context.DriverInfos
                .FirstOrDefaultAsync(d => d.Id == dto.DriverId.Value, cancellationToken);
            if (driver == null)
                throw new NotFoundException(nameof(OLH_DriverInfo), dto.DriverId.Value.ToString());

            // Verify Vehicle exists
            if (!dto.VehicleId.HasValue)
                throw new ValidationException("VehicleId is required when assigning a driver.");

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId.Value, cancellationToken);
            if (vehicle == null)
                throw new NotFoundException(nameof(OLH_Vehicle), dto.VehicleId.Value.ToString());

            // Assign
            bowserRequest.AssignedDriverId = dto.DriverId.Value;
            bowserRequest.AssignedVehicleId = dto.VehicleId.Value;

            // Find the "OnDuty" status id from DriverStatus table
            var onDutyStatusId = await _context.DriverStatuses
                .Where(s => s.Status == "OnDuty")
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (onDutyStatusId == Guid.Empty)
                throw new NotFoundException(nameof(DriverStatus), "OnDuty");

            // Update DriverInfo to point to that status
            driver.DriverStatusId = onDutyStatusId;

            message += "Driver and vehicle updated successfully. ";
        }
        else
        {

            if (bowserRequest.AssignedDriverId.HasValue && bowserRequest.AssignedVehicleId.HasValue)
            {
                // Verify Driver exists
                var oldDriver = await _context.DriverInfos
                    .FirstOrDefaultAsync(d => d.Id == bowserRequest.AssignedDriverId.Value, cancellationToken);

                if (oldDriver == null)
                    throw new NotFoundException(nameof(OLH_DriverInfo), dto.DriverId?.ToString() ?? "Driver");

                // Verify Vehicle exists
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == bowserRequest.AssignedVehicleId.Value, cancellationToken);
                if (vehicle == null)
                    throw new NotFoundException(nameof(OLH_Vehicle), dto.VehicleId?.ToString() ?? "Vehicle");

                //  Both Driver and Vehicle become null
                bowserRequest.AssignedDriverId = null;
                bowserRequest.AssignedVehicleId = null;

                // Find the "Available" status id from DriverStatus table
                var availableStatusId = await _context.DriverStatuses
                    .Where(s => s.Status == "Available")
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (availableStatusId == Guid.Empty)
                    throw new NotFoundException(nameof(DriverStatus), "Available");

                // Update DriverInfo to point to that status
                oldDriver.DriverStatusId = availableStatusId;

                message += "Driver and vehicle unassigned successfully. ";
            }
            else if(!bowserRequest.AssignedDriverId.HasValue && !bowserRequest.AssignedVehicleId.HasValue)
            {
                throw new Exception("Driver and Vehicle already un-assigned/ null");
            }
        }

        // Case 2: Planned Delivery Date update (can be null or new value)
        if (dto.PlannedDeliveryDate.HasValue || bowserRequest.PlannedDeliveryDate != null)
        {
            bowserRequest.PlannedDeliveryDate = dto.PlannedDeliveryDate;
            message += "Planned delivery date updated successfully. ";
        }
        else
        {
            bowserRequest.PlannedDeliveryDate = null;
            message += "Planned delivery date removed successfully. ";
        }

        _context.BowserRequests.Update(bowserRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(bowserRequest.Id, message.Trim());
    }
}


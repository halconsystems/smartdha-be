using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.DriverShift.Commands;
public record AddDriverShiftCommand(Guid VehicleId, Guid DriverId, Guid ShiftId, DateOnly DutyDate)
    : IRequest<SuccessResponse<string>>;

public class AddDriverShiftHandler : IRequestHandler<AddDriverShiftCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddDriverShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddDriverShiftCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var alreadyExists = await _context.DriverShifts
            .AnyAsync(ds =>
                ds.VehicleId == request.VehicleId &&
                ds.DriverId == request.DriverId &&
                ds.ShiftId == request.ShiftId &&
                ds.DutyDate == request.DutyDate &&
                ds.IsDeleted == false && ds.IsActive == true,
                cancellationToken
            );

            if (alreadyExists)
            {
                return new SuccessResponse<string>(
                    $"A driver shift already exists for Vehicle, Driver, Shift and Date ({request.DutyDate}).");
            }

            var entity = new OLH_DriverShift
            {
                VehicleId = request.VehicleId,
                DriverId = request.DriverId,
                ShiftId = request.ShiftId,
                DutyDate = request.DutyDate,
                IsActive = true,
                IsDeleted = false
            };

            _context.DriverShifts.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new SuccessResponse<string>($"Driver shift for duty date {request.DutyDate} added successfully.");
        }
        catch (Exception ex)
        {
            // Log the exception (you can use a logging framework here)
            return new SuccessResponse<string>($"Error adding driver shift: {ex.Message}");
        }
    }
}

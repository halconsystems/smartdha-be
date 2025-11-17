using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DriverShift.Commands;
public record UpdateDriverShiftCommand(Guid Id, Guid VehicleId, Guid DriverId, Guid ShiftId, DateOnly DutyDate)
    : IRequest<SuccessResponse<string>>;

public class UpdateDriverShiftHandler : IRequestHandler<UpdateDriverShiftCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateDriverShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateDriverShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DriverShifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Driver shift with Id '{request.Id}' not found.");

        var duplicateExists = await _context.DriverShifts
        .AnyAsync(ds =>
            ds.Id != request.Id &&                     // exclude current record
            ds.VehicleId == request.VehicleId &&
            ds.DriverId == request.DriverId &&
            ds.ShiftId == request.ShiftId &&
            ds.DutyDate == request.DutyDate &&
            ds.IsDeleted == false && ds.IsActive == true,
            cancellationToken
        );

        if (duplicateExists)
        {
            return new SuccessResponse<string>(
                $"A driver shift already exists for this Vehicle, Driver, Shift and Date ({request.DutyDate}). Update canceled.");
        }


        entity.VehicleId = request.VehicleId;
        entity.DriverId = request.DriverId;
        entity.ShiftId = request.ShiftId;
        entity.DutyDate = request.DutyDate;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver shift for duty date {request.DutyDate} updated successfully.");
    }
}

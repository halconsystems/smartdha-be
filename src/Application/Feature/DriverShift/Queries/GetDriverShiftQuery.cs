using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DriverShift.Queries;
public record GetDriverShiftQuery(Guid? Id)
    : IRequest<SuccessResponse<List<DriverShiftDto>>>;

public class GetDriverShiftHandler : IRequestHandler<GetDriverShiftQuery, SuccessResponse<List<DriverShiftDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetDriverShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<DriverShiftDto>>> Handle(GetDriverShiftQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DriverShifts
            .Include(s => s.Vehicle)
            .Include(s => s.DriverInfo)
            .Include(s => s.Shift)
            .Where(s => s.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var shift = await query.FirstOrDefaultAsync(s => s.Id == request.Id.Value, cancellationToken);
            if (shift == null)
                throw new ArgumentException($"Driver shift with Id '{request.Id}' not found.");

            return new SuccessResponse<List<DriverShiftDto>>(new List<DriverShiftDto>
            {
                new DriverShiftDto
                {
                    Id = shift.Id,
                    VehicleId = shift.VehicleId,
                    VehiclePlateNo = shift.Vehicle.LicensePlateNumber,
                    DriverId = shift.DriverId,
                    DriverName = shift.DriverInfo.DriverName,
                    ShiftId = shift.ShiftId,
                    ShiftName = shift.Shift.ShiftName,
                    DutyDate = shift.DutyDate,
                    IsActive = shift.IsActive
                }
            });
        }

        var shifts = await query.ToListAsync(cancellationToken);
        var dtoList = shifts.Select(s => new DriverShiftDto
        {
            Id = s.Id,
            VehicleId = s.VehicleId,
            VehiclePlateNo = s.Vehicle.LicensePlateNumber,
            DriverId = s.DriverId,
            DriverName = s.DriverInfo.DriverName,
            ShiftId = s.ShiftId,
            ShiftName = s.Shift.ShiftName,
            DutyDate = s.DutyDate,
            IsActive = s.IsActive
        }).ToList();

        return new SuccessResponse<List<DriverShiftDto>>(dtoList);
    }
}

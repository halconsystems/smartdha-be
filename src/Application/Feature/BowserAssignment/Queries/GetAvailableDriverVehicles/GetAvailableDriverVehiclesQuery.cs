using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAvailableDriverVehicles;

public record GetAvailableDriverVehiclesQuery(DateTime RequestedDeliveryDate, Guid PhaseId, Guid BowserCapacityId)
    : IRequest<List<AvailableDriverVehicleDto>>;
public class GetAvailableDriverVehiclesHandler : IRequestHandler<GetAvailableDriverVehiclesQuery, List<AvailableDriverVehicleDto>>
{
    private readonly IOLHApplicationDbContext _context;

    public GetAvailableDriverVehiclesHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailableDriverVehicleDto>> Handle(GetAvailableDriverVehiclesQuery request, CancellationToken cancellationToken)
    {
        var requestDate = DateOnly.FromDateTime(request.RequestedDeliveryDate);
        var requestTime = TimeOnly.FromDateTime(request.RequestedDeliveryDate);

        var query = await _context.DriverShifts
            .Include(ds => ds.DriverInfo)
                .ThenInclude(d => d.DriverStatus)
            .Include(ds => ds.Vehicle)
                .ThenInclude(v => v.Make)
            .Include(ds => ds.Vehicle)
                .ThenInclude(v => v.Model)
            .Include(ds => ds.Vehicle)
                .ThenInclude(v => v.VehicleType)
            .Include(ds => ds.Vehicle)
                .ThenInclude(v => v.VehicleOwner)
            .Include(ds => ds.Shift)
            .Where(ds =>
                ds.DutyDate == requestDate &&
                ds.DriverInfo.DriverStatus.Status == Domain.Enums.DriverStatus.Available &&
                ds.Vehicle.VehicleStatus.Status == VehicleStatus.Active &&
                ds.Vehicle.BowserCapacityId == request.BowserCapacityId &&
                requestTime >= ds.Shift.StartTime &&   // requested delivery time must lie b/w driver's shift timings
                requestTime <= ds.Shift.EndTime
            )
            .Select(ds => new AvailableDriverVehicleDto
            {
                DriverId = ds.DriverInfo.Id,
                DriverName = ds.DriverInfo.DriverName,
                CNIC = ds.DriverInfo.CNIC,
                MobileNo = ds.DriverInfo.MobileNo,
                Email = ds.DriverInfo.Email,
                VehicleId = ds.Vehicle.Id,
                LicensePlateNumber = ds.Vehicle.LicensePlateNumber,
                VehicleMake = ds.Vehicle.Make != null ? ds.Vehicle.Make.MakeName : "",
                VehicleModel = ds.Vehicle.Model != null ? ds.Vehicle.Model.ModelName : "",
                VehicleType = ds.Vehicle.VehicleType.TypeName,
                VehicleOwner = ds.Vehicle.VehicleOwner.OwnerName
            })
            .ToListAsync(cancellationToken);

        return query;
    }
}


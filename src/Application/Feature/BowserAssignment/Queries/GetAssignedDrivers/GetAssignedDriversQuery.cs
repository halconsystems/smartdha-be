using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAssignedDrivers;
public record GetAssignedDriversQuery() : IRequest<List<AssignedDriverDto>>;

public class GetAssignedDriversHandler : IRequestHandler<GetAssignedDriversQuery, List<AssignedDriverDto>>
{
    private readonly IOLHApplicationDbContext _context;

    public GetAssignedDriversHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AssignedDriverDto>> Handle(GetAssignedDriversQuery request, CancellationToken cancellationToken)
    {
        var query = await _context.BowserRequests
            .Include(br => br.AssignedDriver)
            .Include(br => br.AssignedVehicle)!.ThenInclude(v => v!.Make)
            .Include(br => br.AssignedVehicle)!.ThenInclude(v => v!.Model)
            .Include(br => br.AssignedVehicle)!.ThenInclude(v => v!.VehicleType)
            .Include(br => br.AssignedVehicle)!.ThenInclude(v => v!.VehicleOwner)
            .Include(br => br.Phase)
            .Where(br => br.AssignedDriverId != null && br.AssignedVehicleId != null) // only assigned
            .Select(br => new AssignedDriverDto
            {
                DriverId = br.AssignedDriverId!.Value,
                DriverName = br.AssignedDriver!.DriverName,
                CNIC = br.AssignedDriver.CNIC,
                MobileNo = br.AssignedDriver.MobileNo,
                Email = br.AssignedDriver.Email,

                VehicleId = br.AssignedVehicleId!.Value,
                LicensePlateNumber = br.AssignedVehicle!.LicensePlateNumber,
                VehicleMake = br.AssignedVehicle.Make != null ? br.AssignedVehicle.Make.MakeName : "",
                VehicleModel = br.AssignedVehicle.Model != null ? br.AssignedVehicle.Model.ModelName : "",
                VehicleType = br.AssignedVehicle.VehicleType.TypeName,
                VehicleOwner = br.AssignedVehicle.VehicleOwner.OwnerName,

                RequestNo = br.RequestNo,
                RequestDate = br.RequestDate,
                Phase = br.Phase != null ? br.Phase.Name : "",
                Ext = br.Ext ?? "",
                Street = br.Street ?? "",
                Address = br.Address ?? "",
                Latitude = (double)(br.Latitude),  
                Longitude = (double)(br.Longitude),
                RequestedDeliveryDate = br.RequestedDeliveryDate,
                PlannedDeliveryDate = br.PlannedDeliveryDate,
                DeliveryDate = br.DeliveryDate
            })
            .ToListAsync(cancellationToken);

        return query;
    }
}

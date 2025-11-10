using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries;
public record GetBowsersQuery(Guid? Id) : IRequest<SuccessResponse<List<BowserDto>>>;

public class GetBowserQueryHandler : IRequestHandler<GetBowsersQuery, SuccessResponse<List<BowserDto>>>
{
    private readonly IOLHApplicationDbContext _context;

    public GetBowserQueryHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<BowserDto>>> Handle(GetBowsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Vehicles
             .Include(v => v.VehicleOwner)
             .Include(v => v.VehicleType)
             .Include(v => v.VehicleStatus)
             .Include(v => v.BowserCapacity)
             .AsQueryable();


        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var vehicle = await query.FirstOrDefaultAsync(v => v.Id == request.Id.Value, cancellationToken);

            if (vehicle == null)
            {
                throw new ArgumentException($"Vehicle with Id '{request.Id}' not found.");
            }

            return new SuccessResponse<List<BowserDto>>(new List<BowserDto> { new BowserDto
                {
                    Id = vehicle.Id,
                    LicensePlateNumber = vehicle.LicensePlateNumber,
                    EngineNumber = vehicle.EngineNumber,
                    ChassisNumber = vehicle.ChassisNumber,
                    MakeId = vehicle.MakeId,
                    ModelId = vehicle.ModelId,
                    YearOfManufacture = vehicle.YearOfManufacture,
                    VehicleOwnerId = vehicle.VehicleOwnerId,
                    VehicleOwnerName = vehicle.VehicleOwner?.OwnerName ?? string.Empty,
                    VehicleTypeId = vehicle.VehicleTypeId,
                    VehicleTypeName = vehicle.VehicleType?.TypeName ?? string.Empty,
                    VehicleStatusId = vehicle.VehicleStatusId,
                    VehicleStatusName = vehicle.VehicleStatus?.Status.ToString() ?? string.Empty,
                    BowserCapacityId = vehicle.BowserCapacityId,
                    BowserCapacityName = vehicle.BowserCapacity?.Capacity.ToString() ?? string.Empty,
                    BowserCapacityUnit = vehicle.BowserCapacity?.Unit ?? string.Empty,
                    Remarks = vehicle.Remarks
                } });
        }

        var vehicles = await query.ToListAsync(cancellationToken);
        return new SuccessResponse<List<BowserDto>>(vehicles.Select(v => new BowserDto
        {
            Id = v.Id,
            LicensePlateNumber = v.LicensePlateNumber,
            EngineNumber = v.EngineNumber,
            ChassisNumber = v.ChassisNumber,
            MakeId = v.MakeId,
            ModelId = v.ModelId,
            YearOfManufacture = v.YearOfManufacture,
            VehicleOwnerId = v.VehicleOwnerId,
            VehicleOwnerName = v.VehicleOwner?.OwnerName ?? string.Empty,
            VehicleTypeId = v.VehicleTypeId,
            VehicleTypeName = v.VehicleType?.TypeName ?? string.Empty,
            VehicleStatusId = v.VehicleStatusId,
            VehicleStatusName = v.VehicleStatus?.Status.ToString() ?? string.Empty,
            BowserCapacityId = v.BowserCapacityId,
            BowserCapacityName = v.BowserCapacity?.Capacity.ToString() ?? string.Empty,
            BowserCapacityUnit = v.BowserCapacity?.Unit ?? string.Empty,
            Remarks = v.Remarks
        }).ToList());

    }
}



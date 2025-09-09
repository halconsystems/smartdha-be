using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Commands;
public record AddBowserCommand(
    string LicensePlateNumber,
    string EngineNumber,
    string ChassisNumber,
    Guid? MakeId,
    Guid? ModelId,
    string YearOfManufacture,
    Guid VehicleOwnerId,
    Guid VehicleTypeId,
    Guid VehicleStatusId,
    Guid BowserCapacityId,
    string? Remarks
) : IRequest<SuccessResponse<string>>;
public class AddBowserCommandHandler : IRequestHandler<AddBowserCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;

    public AddBowserCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddBowserCommand request, CancellationToken cancellationToken)
    {
        var entity = new OLH_Vehicle
        {
            LicensePlateNumber = request.LicensePlateNumber,
            EngineNumber = request.EngineNumber,
            ChassisNumber = request.ChassisNumber,
            MakeId = request.MakeId,
            ModelId = request.ModelId,
            YearOfManufacture = request.YearOfManufacture,
            VehicleOwnerId = request.VehicleOwnerId,
            VehicleTypeId = request.VehicleTypeId,
            VehicleStatusId = request.VehicleStatusId,
            BowserCapacityId = request.BowserCapacityId,
            Remarks = request.Remarks
        };

        _context.Vehicles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle '{request.LicensePlateNumber}' successfully added.");
    }
}




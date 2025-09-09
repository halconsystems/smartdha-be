using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Commands;
public record UpdateBowserCommand(
    Guid Id,
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

public class UpdateBowserCommandHandler : IRequestHandler<UpdateBowserCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;

    public UpdateBowserCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateBowserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Vehicles.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new ArgumentException($"Vehicle with Id '{request.Id}' not found.");
        }

        entity.LicensePlateNumber = request.LicensePlateNumber;
        entity.EngineNumber = request.EngineNumber;
        entity.ChassisNumber = request.ChassisNumber;
        entity.MakeId = request.MakeId;
        entity.ModelId = request.ModelId;
        entity.YearOfManufacture = request.YearOfManufacture;
        entity.VehicleOwnerId = request.VehicleOwnerId;
        entity.VehicleTypeId = request.VehicleTypeId;
        entity.VehicleStatusId = request.VehicleStatusId;
        entity.BowserCapacityId = request.BowserCapacityId;
        entity.Remarks = request.Remarks;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle '{request.LicensePlateNumber}' successfully updated.");
    }
}



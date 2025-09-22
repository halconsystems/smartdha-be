using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
public record AddVehicleTypeCommand(string TypeName) : IRequest<SuccessResponse<string>>;

public class AddVehicleTypeHandler : IRequestHandler<AddVehicleTypeCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddVehicleTypeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddVehicleTypeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.VehicleTypes
            .AnyAsync(x => x.TypeName == request.TypeName && x.IsDeleted != true, cancellationToken);

        if (exists)
            throw new ArgumentException($"Vehicle Type '{request.TypeName}' already exists.");

        var entity = new OLH_VehicleType { TypeName = request.TypeName };
        _context.VehicleTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle Type '{request.TypeName}' successfully added.");
    }
}

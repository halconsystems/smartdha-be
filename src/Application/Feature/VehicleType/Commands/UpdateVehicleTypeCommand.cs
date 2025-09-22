using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;

public record UpdateVehicleTypeCommand(Guid Id, string TypeName) : IRequest<SuccessResponse<string>>;

public class UpdateVehicleTypeHandler : IRequestHandler<UpdateVehicleTypeCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateVehicleTypeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateVehicleTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Vehicle Type '{request.Id}' not found.");

        // Optionally check duplication with other types
        var dupe = await _context.VehicleTypes
            .AnyAsync(x => x.Id != request.Id && x.TypeName == request.TypeName && x.IsDeleted != true, cancellationToken);
        if (dupe) throw new ArgumentException($"Another Vehicle Type with name '{request.TypeName}' already exists.");

        entity.TypeName = request.TypeName;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle Type '{request.TypeName}' updated successfully.");
    }
}

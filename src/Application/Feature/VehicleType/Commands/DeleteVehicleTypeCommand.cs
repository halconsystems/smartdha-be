using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
public record DeleteVehicleTypeCommand(Guid Id) : IRequest<SuccessResponse<string>>;

public class DeleteVehicleTypeHandler : IRequestHandler<DeleteVehicleTypeCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteVehicleTypeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteVehicleTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Vehicle Type '{request.Id}' not found.");

        // Soft delete
        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle Type '{entity.TypeName}' deleted (soft).");
    }
}

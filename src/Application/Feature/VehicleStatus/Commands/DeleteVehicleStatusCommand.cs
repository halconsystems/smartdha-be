using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
public record DeleteVehicleStatusCommand(Guid Id) : IRequest<SuccessResponse<string>>;

public class DeleteVehicleStatusHandler : IRequestHandler<DeleteVehicleStatusCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteVehicleStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleStatuses
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Vehicle Status '{request.Id}' not found.");

        // Soft delete
        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle Status '{entity.Status}' deleted (soft).");
    }
}

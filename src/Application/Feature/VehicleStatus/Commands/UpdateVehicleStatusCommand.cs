using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;

public record UpdateVehicleStatusCommand(Guid Id, VehicleStatus Status) : IRequest<SuccessResponse<string>>;

public class UpdateVehicleStatusHandler : IRequestHandler<UpdateVehicleStatusCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateVehicleStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleStatuses
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Vehicle Status '{request.Id}' not found.");

        // Optionally check duplication with other types
        var dupe = await _context.VehicleStatuses
            .AnyAsync(x => x.Id != request.Id && x.Status == request.Status && x.IsDeleted != true, cancellationToken);
        if (dupe) throw new ArgumentException($"Another Vehicle Status with name '{request.Status}' already exists.");

        entity.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle Status '{request.Status}' updated successfully.");
    }
}

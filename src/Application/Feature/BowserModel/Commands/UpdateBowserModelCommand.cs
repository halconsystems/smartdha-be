using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserModel.Commands;

public record UpdateBowserModelCommand(Guid Id, Guid MakeId, string ModelName) : IRequest<SuccessResponse<string>>;

public class UpdateBowserModelHandler : IRequestHandler<UpdateBowserModelCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateBowserModelHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateBowserModelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleModels
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted!=true, cancellationToken);
        if (entity == null) throw new ArgumentException($"Bowser Model '{request.Id}' not found.");

        // ensure make exists and is not deleted
        var make = await _context.VehicleMakes
            .FirstOrDefaultAsync(m => m.Id == request.MakeId && m.IsDeleted!=true, cancellationToken);
        if (make == null) throw new ArgumentException($"Bowser Make '{request.MakeId}' not found.");

        // duplicate check (other models)
        var dupe = await _context.VehicleModels
            .AnyAsync(x => x.Id != request.Id && x.MakeId == request.MakeId && x.ModelName == request.ModelName && x.IsDeleted!=true, cancellationToken);
        if (dupe) throw new ArgumentException($"Another Model with name '{request.ModelName}' already exists for the selected make.");

        entity.ModelName = request.ModelName;
        entity.MakeId = request.MakeId;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Model '{request.ModelName}' updated successfully.");
    }
}

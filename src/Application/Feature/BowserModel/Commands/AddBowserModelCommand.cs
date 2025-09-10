using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserModel.Commands;

public record AddBowserModelCommand(Guid MakeId, string ModelName) : IRequest<SuccessResponse<string>>;
public class AddBowserModelHandler : IRequestHandler<AddBowserModelCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddBowserModelHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddBowserModelCommand request, CancellationToken cancellationToken)
    {
        // ensure make exists and is not deleted
        var make = await _context.VehicleMakes
            .FirstOrDefaultAsync(m => m.Id == request.MakeId && m.IsDeleted!=true, cancellationToken);
        if (make == null) throw new ArgumentException($"Bowser Make '{request.MakeId}' not found.");

        // check duplicate model for same make
        var exists = await _context.VehicleModels
            .AnyAsync(x => x.MakeId == request.MakeId && x.ModelName == request.ModelName && x.IsDeleted!=true, cancellationToken);
        if (exists) throw new ArgumentException($"Model '{request.ModelName}' already exists for the selected make.");

        var entity = new OLH_VehicleModel
        {
            MakeId = request.MakeId,
            ModelName = request.ModelName
        };

        _context.VehicleModels.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Model '{request.ModelName}' successfully added.");
    }
}

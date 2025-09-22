using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;

public record UpdateBowserMakeCommand(Guid Id, string MakeName) : IRequest<SuccessResponse<string>>;

//public class UpdateBowserMakeHandler : IRequestHandler<UpdateBowserMakeCommand, SuccessResponse<string>>
//{
//    private readonly IOLHApplicationDbContext _context;
//    public UpdateBowserMakeHandler(IOLHApplicationDbContext context) => _context = context;

//    public async Task<SuccessResponse<string>> Handle(UpdateBowserMakeCommand request, CancellationToken cancellationToken)
//    {
//        var entity = await _context.BowserMakes.FindAsync(new object[] { request.Id }, cancellationToken);
//        if (entity == null) throw new ArgumentException($"Bowser Make '{request.Id}' not found.");

//        entity.MakeName = request.MakeName;
//        await _context.SaveChangesAsync(cancellationToken);

//        return new SuccessResponse<string>($"Bowser Make '{request.MakeName}' updated successfully.");
//    }
//}
public class UpdateBowserMakeHandler : IRequestHandler<UpdateBowserMakeCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateBowserMakeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateBowserMakeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleMakes
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted!=true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Bowser Make '{request.Id}' not found.");

        // Optionally check duplication with other makes
        var dupe = await _context.VehicleMakes
            .AnyAsync(x => x.Id != request.Id && x.MakeName == request.MakeName && x.IsDeleted!=true, cancellationToken);
        if (dupe) throw new ArgumentException($"Another Bowser Make with name '{request.MakeName}' already exists.");

        entity.MakeName = request.MakeName;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Make '{request.MakeName}' updated successfully.");
    }
}

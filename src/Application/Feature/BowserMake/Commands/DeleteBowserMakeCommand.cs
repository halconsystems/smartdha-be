using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
public record DeleteBowserMakeCommand(Guid Id) : IRequest<SuccessResponse<string>>;

//public class DeleteBowserMakeHandler : IRequestHandler<DeleteBowserMakeCommand, SuccessResponse<string>>
//{
//    private readonly IOLHApplicationDbContext _context;
//    public DeleteBowserMakeHandler(IOLHApplicationDbContext context) => _context = context;

//    public async Task<SuccessResponse<string>> Handle(DeleteBowserMakeCommand request, CancellationToken cancellationToken)
//    {
//        var entity = await _context.BowserMakes.FindAsync(new object[] { request.Id }, cancellationToken);
//        if (entity == null) throw new ArgumentException($"Bowser Make '{request.Id}' not found.");

//        _context.BowserMakes.Remove(entity);
//        await _context.SaveChangesAsync(cancellationToken);

//        return new SuccessResponse<string>($"Bowser Make '{entity.MakeName}' deleted successfully.");
//    }
//}
public class DeleteBowserMakeHandler : IRequestHandler<DeleteBowserMakeCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteBowserMakeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteBowserMakeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleMakes
            .Include(x => x.Models)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Bowser Make '{request.Id}' not found.");

        // Prevent deleting a make that still has non-deleted models (optional but recommended)
        var hasActiveModels = entity.Models.Any(m => m.IsDeleted != true);
        if (hasActiveModels) throw new InvalidOperationException("Cannot delete make with existing models. Delete or reassign the models first.");

        // Soft delete
        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Make '{entity.MakeName}' deleted (soft).");
    }
}

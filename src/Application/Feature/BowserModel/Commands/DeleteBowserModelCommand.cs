using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserModel.Commands;

public record DeleteBowserModelCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteBowserModelHandler : IRequestHandler<DeleteBowserModelCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteBowserModelHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteBowserModelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleModels
            .Include(x => x.Make)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted!=true, cancellationToken);
        if (entity == null) throw new ArgumentException($"Bowser Model '{request.Id}' not found.");

        // Soft delete
        entity.IsDeleted = true;
        
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Model '{entity.ModelName}' deleted (soft).");
    }
}

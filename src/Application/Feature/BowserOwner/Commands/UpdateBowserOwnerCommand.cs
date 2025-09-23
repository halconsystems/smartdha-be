using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserOwner.Commands;

public record UpdateBowserOwnerCommand(Guid Id, string OwnerName) : IRequest<SuccessResponse<string>>;

// UPDATE
public class UpdateBowserOwnerHandler : IRequestHandler<UpdateBowserOwnerCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateBowserOwnerHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateBowserOwnerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleOwners
            .FirstOrDefaultAsync(o => o.Id == request.Id && o.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Bowser Owner with Id '{request.Id}' not found.");

        entity.OwnerName = request.OwnerName;
        

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Owner '{entity.OwnerName}' updated successfully.");
    }
}

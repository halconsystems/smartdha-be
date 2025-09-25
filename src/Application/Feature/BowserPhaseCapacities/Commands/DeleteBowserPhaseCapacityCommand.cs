using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserPhaseCapacities.Commands;
public class DeleteBowserPhaseCapacityCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
}

public class DeleteBowserPhaseCapacityCommandHandler : IRequestHandler<DeleteBowserPhaseCapacityCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
   

    public DeleteBowserPhaseCapacityCommandHandler(IOLHApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
       
    }

    public async Task<SuccessResponse<string>> Handle(DeleteBowserPhaseCapacityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PhaseCapacities
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false), cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(OLH_PhaseCapacity), request.Id.ToString());
        }

        entity.IsDeleted = true;
  

        await _context.SaveChangesAsync(cancellationToken);
        return new SuccessResponse<string>($"Phase Capacity Id {request.Id} marked as deleted.");
    }
}

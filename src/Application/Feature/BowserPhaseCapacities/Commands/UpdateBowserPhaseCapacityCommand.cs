using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserPhaseCapacities.Commands;
public class UpdateBowserPhaseCapacityCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
    public Guid PhaseId { get; set; }
    public Guid BowserCapacityId { get; set; }
    public decimal? BaseRate { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EffectiveTo { get; set; }
}

public class UpdateBowserPhaseCapacityCommandHandler : IRequestHandler<UpdateBowserPhaseCapacityCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
  

    public UpdateBowserPhaseCapacityCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
     
    }

    public async Task<SuccessResponse<string>> Handle(UpdateBowserPhaseCapacityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PhaseCapacities
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false), cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(OLH_PhaseCapacity), request.Id.ToString());
        }
        entity.PhaseId = request.PhaseId;
        entity.BowserCapacityId = request.BowserCapacityId;
        entity.BaseRate = request.BaseRate;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Phase Capacity Id {request.Id} data successfully updated.");
    }
}



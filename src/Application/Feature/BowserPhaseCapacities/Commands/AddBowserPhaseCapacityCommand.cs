using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;

public class AddBowserPhaseCapacityCommand : IRequest<SuccessResponse<string>>
{
    public Guid PhaseId { get; set; }
    public Guid BowserCapacityId { get; set; }
    public decimal? BaseRate { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EffectiveTo { get; set; }
}

public class AddBowserPhaseCapacityCommandHandler : IRequestHandler<AddBowserPhaseCapacityCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;

    public AddBowserPhaseCapacityCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddBowserPhaseCapacityCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.PhaseCapacities
            .AnyAsync(c => c.BowserCapacityId == request.BowserCapacityId && c.PhaseId == request.PhaseId 
            && c.IsDeleted != true, cancellationToken);

        if (exists)
        {
            throw new ArgumentException($"Phase Capacity {request.PhaseId} + {request.BowserCapacityId} already exists.");
        }

        var entity = new OLH_PhaseCapacity
        {
            PhaseId = request.PhaseId,
            BowserCapacityId = request.BowserCapacityId,
            BaseRate = request.BaseRate,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo
        };

        _context.PhaseCapacities.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Phase Capacity {request.PhaseId} + {request.BowserCapacityId} successfully added.");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
public class GetBowserPhaseCapacityQuery : IRequest<SuccessResponse<object>>
{
    public Guid? Id { get; set; }
       
}

public class GetBowserPhaseCapacityQueryHandler : IRequestHandler<GetBowserPhaseCapacityQuery, SuccessResponse<object>>
{
    private readonly IOLHApplicationDbContext _context;

    public GetBowserPhaseCapacityQueryHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<object>> Handle(GetBowserPhaseCapacityQuery request, CancellationToken cancellationToken)
    {

        if (request.Id.HasValue)
        {
            var entity = await _context.PhaseCapacities
                .Where(x => x.Id == request.Id.Value && (x.IsDeleted == null || x.IsDeleted == false))
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(OLH_PhaseCapacity), request?.Id?.ToString()?? "No ID provided");
            }

            var dto = new BowserPhaseCapacityDto
            {
                Id = entity.Id,
                PhaseId = entity.PhaseId,
                BowserCapacityId = entity.BowserCapacityId,
                BaseRate = entity.BaseRate,
                EffectiveFrom = entity.EffectiveFrom,
                EffectiveTo = entity.EffectiveTo
            };

            return new SuccessResponse<object>(dto, "Phase capacity retrieved.");
        }
        else
        {
            var entities = await _context.PhaseCapacities
                .Where(x => x.IsDeleted == null || x.IsDeleted == false)
                .ToListAsync(cancellationToken);

            var dtoList = entities.Select(entity => new BowserPhaseCapacityDto
            {
                Id = entity.Id,
                PhaseId = entity.PhaseId,
                BowserCapacityId = entity.BowserCapacityId,
                BaseRate = entity.BaseRate,
                EffectiveFrom = entity.EffectiveFrom,
                EffectiveTo = entity.EffectiveTo
            }).ToList();

            return new SuccessResponse<object>(dtoList, "All active phase capacities retrieved.");
        }
    }

}


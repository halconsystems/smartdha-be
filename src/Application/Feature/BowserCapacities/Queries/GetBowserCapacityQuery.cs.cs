using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
public class GetBowserCapacityQuery : IRequest<SuccessResponse<object>>
{
    public Guid? Id { get; set; }
       
}

public class GetBowserCapacityQueryHandler : IRequestHandler<GetBowserCapacityQuery, SuccessResponse<object>>
{
    private readonly IOLHApplicationDbContext _context;

    public GetBowserCapacityQueryHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<object>> Handle(GetBowserCapacityQuery request, CancellationToken cancellationToken)
    {

        if (request.Id.HasValue)
        {
            var entity = await _context.BowserCapacitys
                .Where(x => x.Id == request.Id.Value && (x.IsDeleted == null || x.IsDeleted == false))
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(OLH_BowserCapacity), request?.Id?.ToString()?? "No ID provided");
            }

            var entityRate = await _context.BowserCapacityRates
                .Where(r => r.BowserCapacityId == entity.Id && (r.IsDeleted == null || r.IsDeleted == false))
                .FirstOrDefaultAsync(cancellationToken);

            var dto = new BowserCapacityDto
            {
                Id = entity.Id,
                Capacity = entity.Capacity,
                Unit = entity.Unit,
                Rate = entityRate?.Rate
            };

            return new SuccessResponse<object>(dto, "Bowser capacity with rate retrieved.");
        }
        else
        {
            var entities = await _context.BowserCapacitys
                .Where(x => x.IsDeleted == null || x.IsDeleted == false)
                .ToListAsync(cancellationToken);

            if (!entities.Any())
                throw new NotFoundException(nameof(OLH_BowserCapacity), "No active capacities found.");

            var capacityIds = entities.Select(e => e.Id).ToList();

            var rates = await _context.BowserCapacityRates
                .Where(r => capacityIds.Contains(r.BowserCapacityId) && (r.IsDeleted == null || r.IsDeleted == false))
                .ToListAsync(cancellationToken);

            var dtoList = entities.Select(entity =>
            {
                var entityRate = rates.FirstOrDefault(r => r.BowserCapacityId == entity.Id);
                return new BowserCapacityDto
                {
                    Id = entity.Id,
                    Capacity = entity.Capacity,
                    Unit = entity.Unit,
                    Rate = entityRate?.Rate
                };
            }).ToList();


            return new SuccessResponse<object>(dtoList, "All active bowser capacities with rates retrieved.");
        }
    }

}


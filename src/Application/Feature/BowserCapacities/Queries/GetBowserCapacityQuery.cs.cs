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

            var dto = new BowserCapacityDto
            {
                Id = entity.Id,
                Capacity = entity.Capacity,
                Unit = entity.Unit
            };

            return new SuccessResponse<object>(dto, "Bowser capacity retrieved.");
        }
        else
        {
            var entities = await _context.BowserCapacitys
                .Where(x => x.IsDeleted == null || x.IsDeleted == false)
                .ToListAsync(cancellationToken);

            var dtoList = entities.Select(entity => new BowserCapacityDto
            {
                Id = entity.Id,
                Capacity = entity.Capacity,
                Unit = entity.Unit
            }).ToList();

            return new SuccessResponse<object>(dtoList, "All active bowser capacities retrieved.");
        }
    }

}


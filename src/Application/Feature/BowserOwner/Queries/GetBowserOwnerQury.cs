using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserOwner.Queries;

public record GetBowserOwnerQuery(Guid? Id) : IRequest<SuccessResponse<List<BowserOwnerDto>>>;

public class GetBowserOwnerHandler : IRequestHandler<GetBowserOwnerQuery, SuccessResponse<List<BowserOwnerDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetBowserOwnerHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<BowserOwnerDto>>> Handle(GetBowserOwnerQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleOwners
            .Where(o => o.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var owner = await query.FirstOrDefaultAsync(o => o.Id == request.Id.Value, cancellationToken);
            if (owner == null)
                throw new ArgumentException($"Bowser Owner with Id '{request.Id}' not found.");

            return new SuccessResponse<List<BowserOwnerDto>>(new List<BowserOwnerDto>
            {
                new BowserOwnerDto
                {
                    Id = owner.Id,
                    OwnerName = owner.OwnerName,
                    IsActive = owner.IsActive
                }
            });
        }

        var owners = await query.ToListAsync(cancellationToken);
        var dtoList = owners.Select(o => new BowserOwnerDto
        {
            Id = o.Id,
            OwnerName = o.OwnerName,
            IsActive = o.IsActive
        }).ToList();

        return new SuccessResponse<List<BowserOwnerDto>>(dtoList);
    }
}

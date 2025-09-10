using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;

public record GetBowserMakesQuery(Guid? Id) : IRequest<SuccessResponse<List<BowserMakeDto>>>;

public class GetBowserMakesHandler : IRequestHandler<GetBowserMakesQuery, SuccessResponse<List<BowserMakeDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetBowserMakesHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<BowserMakeDto>>> Handle(GetBowserMakesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleMakes
            .Where(m => m.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var make = await query.FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);
            if (make == null) throw new ArgumentException($"Bowser Make '{request.Id}' not found.");

            return new SuccessResponse<List<BowserMakeDto>>(new List<BowserMakeDto>
            {
                new BowserMakeDto { Id = make.Id, MakeName = make.MakeName }
            });
        }

        var makes = await query.ToListAsync(cancellationToken);
        return new SuccessResponse<List<BowserMakeDto>>(makes.Select(x => new BowserMakeDto
        {
            Id = x.Id,
            MakeName = x.MakeName
        }).ToList());
    }
}

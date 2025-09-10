using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserModel.Queries;
public record GetBowserModelsQuery(Guid? Id) : IRequest<SuccessResponse<List<BowserModelDto>>>;

public class GetBowserModelsHandler : IRequestHandler<GetBowserModelsQuery, SuccessResponse<List<BowserModelDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetBowserModelsHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<BowserModelDto>>> Handle(GetBowserModelsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleModels
            .Include(m => m.Make)
            .Where(m => m.IsDeleted!=true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var model = await query.FirstOrDefaultAsync(m => m.Id == request.Id.Value, cancellationToken);
            if (model == null) throw new ArgumentException($"Bowser Model '{request.Id}' not found.");

            var single = new BowserModelDto
            {
                Id = model.Id,
                ModelName = model.ModelName,
                MakeId = model.MakeId,
                MakeName = model.Make?.MakeName ?? string.Empty
            };
            return new SuccessResponse<List<BowserModelDto>>(new List<BowserModelDto> { single });
        }

        var models = await query.ToListAsync(cancellationToken);
        var dtoList = models.Select(m => new BowserModelDto
        {
            Id = m.Id,
            ModelName = m.ModelName,
            MakeId = m.MakeId,
            MakeName = m.Make?.MakeName ?? string.Empty
        }).ToList();

        return new SuccessResponse<List<BowserModelDto>>(dtoList);
    }
}

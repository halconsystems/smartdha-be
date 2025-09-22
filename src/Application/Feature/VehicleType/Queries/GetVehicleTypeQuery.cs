using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;

public record GetVehicleTypeQuery(Guid? Id) : IRequest<SuccessResponse<List<VehicleTypeDto>>>;

public class GetVehicleTypeHandler : IRequestHandler<GetVehicleTypeQuery, SuccessResponse<List<VehicleTypeDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetVehicleTypeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<VehicleTypeDto>>> Handle(GetVehicleTypeQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleTypes
            .Where(m => m.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var type = await query.FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);
            if (type == null) throw new ArgumentException($"Vehicle Type '{request.Id}' not found.");

            return new SuccessResponse<List<VehicleTypeDto>>(new List<VehicleTypeDto>
            {
                new VehicleTypeDto { Id = type.Id, TypeName = type.TypeName }
            });
        }

        var types = await query.ToListAsync(cancellationToken);
        return new SuccessResponse<List<VehicleTypeDto>>(types.Select(x => new VehicleTypeDto
        {
            Id = x.Id,
            TypeName = x.TypeName
        }).ToList());
    }
}

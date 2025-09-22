using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;

public record GetVehicleStatusQuery(Guid? Id) : IRequest<SuccessResponse<List<VehicleStatusDto>>>;

public class GetVehicleStatusHandler : IRequestHandler<GetVehicleStatusQuery, SuccessResponse<List<VehicleStatusDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetVehicleStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<VehicleStatusDto>>> Handle(GetVehicleStatusQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VehicleStatuses
            .Where(m => m.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var status = await query.FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);
            if (status == null) throw new ArgumentException($"Vehicle Status '{request.Id}' not found.");

            return new SuccessResponse<List<VehicleStatusDto>>(new List<VehicleStatusDto>
            {
                new VehicleStatusDto { Id = status.Id, Status = status.Status, Remarks = status.Remarks }
            });
        }

        var statuses = await query.ToListAsync(cancellationToken);
        return new SuccessResponse<List<VehicleStatusDto>>(statuses.Select(x => new VehicleStatusDto
        {
            Id = x.Id,
            Status = x.Status,
            Remarks = x.Remarks
        }).ToList());
    }
}

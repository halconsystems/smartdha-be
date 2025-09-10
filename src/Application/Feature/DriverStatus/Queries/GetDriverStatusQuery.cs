using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DriverStatus.Queries;
// Get (by Id or all)
public record GetDriverStatusQuery(Guid? Id) : IRequest<SuccessResponse<List<DriverStatusDto>>>;
// GET
public class GetDriverStatusHandler : IRequestHandler<GetDriverStatusQuery, SuccessResponse<List<DriverStatusDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetDriverStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<DriverStatusDto>>> Handle(GetDriverStatusQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DriverStatuses
            .Where(s => s.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var status = await query.FirstOrDefaultAsync(s => s.Id == request.Id.Value, cancellationToken);
            if (status == null)
                throw new ArgumentException($"Driver status with Id '{request.Id}' not found.");

            return new SuccessResponse<List<DriverStatusDto>>(new List<DriverStatusDto>
            {
                new DriverStatusDto
                {
                    Id = status.Id,
                    Status = status.Status,
                    IsActive = status.IsActive
                }
            });
        }

        var statuses = await query.ToListAsync(cancellationToken);
        var dtoList = statuses.Select(s => new DriverStatusDto
        {
            Id = s.Id,
            Status = s.Status,
            IsActive = s.IsActive
        }).ToList();

        return new SuccessResponse<List<DriverStatusDto>>(dtoList);
    }
}

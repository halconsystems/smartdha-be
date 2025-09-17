using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Shifts.Queries;
public record GetShiftQuery(Guid? Id)
    : IRequest<SuccessResponse<List<ShiftDto>>>;
public class GetShiftHandler : IRequestHandler<GetShiftQuery, SuccessResponse<List<ShiftDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<ShiftDto>>> Handle(GetShiftQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Shifts
            .Where(s => s.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var shift = await query.FirstOrDefaultAsync(s => s.Id == request.Id.Value, cancellationToken);
            if (shift == null)
                throw new ArgumentException($"Shift with Id '{request.Id}' not found.");

            return new SuccessResponse<List<ShiftDto>>(new List<ShiftDto>
            {
                new ShiftDto
                {
                    Id = shift.Id,
                    ShiftName = shift.ShiftName,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    IsActive = shift.IsActive
                }
            });
        }

        var shifts = await query.ToListAsync(cancellationToken);
        var dtoList = shifts.Select(s => new ShiftDto
        {
            Id = s.Id,
            ShiftName = s.ShiftName,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            IsActive = s.IsActive
        }).ToList();

        return new SuccessResponse<List<ShiftDto>>(dtoList);
    }
}

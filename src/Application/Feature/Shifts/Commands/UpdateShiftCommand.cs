using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Shifts.Commands;
public record UpdateShiftCommand(Guid Id, string ShiftName, TimeOnly StartTime, TimeOnly EndTime)
    : IRequest<SuccessResponse<string>>;

public class UpdateShiftHandler : IRequestHandler<UpdateShiftCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Shifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Shift with Id '{request.Id}' not found.");

        entity.ShiftName = request.ShiftName;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Shift '{entity.ShiftName}' updated successfully.");
    }
}

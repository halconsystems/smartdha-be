using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DriverShift.Commands;
public record DeleteDriverShiftCommand(Guid Id)
    : IRequest<SuccessResponse<string>>;

public class DeleteDriverShiftHandler : IRequestHandler<DeleteDriverShiftCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteDriverShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteDriverShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DriverShifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Driver shift with Id '{request.Id}' not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver shift for duty date {entity.DutyDate} deleted successfully (soft delete).");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Shifts.Commands;
public record DeleteShiftCommand(Guid Id)
    : IRequest<SuccessResponse<string>>;

public class DeleteShiftHandler : IRequestHandler<DeleteShiftCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Shifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Shift with Id '{request.Id}' not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Shift '{entity.ShiftName}' deleted successfully (soft delete).");
    }
}

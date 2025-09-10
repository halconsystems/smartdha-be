using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DriverStatus.Commands;
// Delete (soft delete)
public record DeleteDriverStatusCommand(Guid Id) : IRequest<SuccessResponse<string>>;
// DELETE (soft delete)
public class DeleteDriverStatusHandler : IRequestHandler<DeleteDriverStatusCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteDriverStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteDriverStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DriverStatuses
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Driver status with Id '{request.Id}' not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver status '{entity.Status}' deleted successfully (soft delete).");
    }
}

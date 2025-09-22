using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Drivers.Commands;
public record DeleteDriverCommand(Guid Id) : IRequest<SuccessResponse<string>>;
// DELETE (soft delete)
public class DeleteDriverHandler : IRequestHandler<DeleteDriverCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public DeleteDriverHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DriverInfos
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Driver '{request.Id}' not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver '{entity.DriverName}' deleted successfully (soft delete).");
    }
}

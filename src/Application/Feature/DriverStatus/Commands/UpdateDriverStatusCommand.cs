using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DriverStatus.Commands;
// Update
public record UpdateDriverStatusCommand(Guid Id, Domain.Enums.DriverStatus Status) : IRequest<SuccessResponse<string>>;
// UPDATE
public class UpdateDriverStatusHandler : IRequestHandler<UpdateDriverStatusCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateDriverStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateDriverStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DriverStatuses
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

        if (entity == null)
            throw new ArgumentException($"Driver status with Id '{request.Id}' not found.");

        entity.Status = request.Status;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver status '{entity.Status}' updated successfully.");
    }
}

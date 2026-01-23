using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Command;


public record ActiveInActiveSizeCommand(
    [Required] Guid Id,
    bool Acive
) : IRequest<SuccessResponse<Guid>>;
public class ActiveInActiveSizeCommandHandler
    : IRequestHandler<ActiveInActiveSizeCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public ActiveInActiveSizeCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(ActiveInActiveSizeCommand request, CancellationToken ct)
    {

        var existingPhase = await _context.TankerSizes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingPhase == null)
            throw new RecordAlreadyExistException($"Size is Not registered.");

        try
        {
            existingPhase.IsActive = request.Acive;

            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                existingPhase.Id,
                "Size Updated successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Size Updation failed: {ex.Message}", ex);
        }
    }
}






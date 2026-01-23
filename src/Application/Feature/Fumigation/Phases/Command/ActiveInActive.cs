using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;

public record ActiveInActivePhasesCommand(
    [Required] Guid Id,
    bool Acive
) : IRequest<SuccessResponse<Guid>>;
public class ActiveInActivePhasesCommandHandler
    : IRequestHandler<ActiveInActivePhasesCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public ActiveInActivePhasesCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(ActiveInActivePhasesCommand request, CancellationToken ct)
    {

        var existingPhase = await _context.FemPhases
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingPhase == null)
            throw new RecordAlreadyExistException($"Phase is Not registered.");

        try
        {
            existingPhase.IsActive = request.Acive;

            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                existingPhase.Id,
                "Phases Updated successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Phases Updation failed: {ex.Message}", ex);
        }
    }
}




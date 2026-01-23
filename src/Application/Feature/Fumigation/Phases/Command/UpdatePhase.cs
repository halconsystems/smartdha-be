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

public record UpdatePhasesCommand(
    [Required] Guid Id,
    [Required] string Name,
    [Required] string DisplayName
) : IRequest<SuccessResponse<Guid>>;
public class UpdatePhasesCommandHandler
    : IRequestHandler<UpdatePhasesCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdatePhasesCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdatePhasesCommand request, CancellationToken ct)
    {

        var existingPhase = await _context.FemPhases
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingPhase == null)
            throw new RecordAlreadyExistException($"Phase '{request.Name}' is Not registered.");

        try
        {
            existingPhase.Name = request.Name;
            existingPhase.DisplayName = request.DisplayName;
            existingPhase.Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper();

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




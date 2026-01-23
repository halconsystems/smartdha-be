using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Command;


public record ActiveInActiveServiceCommand(
    [Required] Guid Id,
    bool Acive
) : IRequest<SuccessResponse<Guid>>;
public class ActiveInActiveServiceCommandHandler
    : IRequestHandler<ActiveInActiveServiceCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public ActiveInActiveServiceCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(ActiveInActiveServiceCommand request, CancellationToken ct)
    {

        var existingPhase = await _context.FemServices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingPhase == null)
            throw new RecordAlreadyExistException($"Service is Not registered.");

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





using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;

public record AddPhasesCommand(
    [Required] string Name,
    [Required] string DisplayName
) : IRequest<SuccessResponse<Guid>>;
public class AddPhasesCommandHandler
    : IRequestHandler<AddPhasesCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public AddPhasesCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddPhasesCommand request, CancellationToken ct)
    {

        var existingPhase = await _context.FemPhases
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Name == request.Name &&
                    u.DisplayName == request.DisplayName,
                    ct);

        if (existingPhase != null)
            throw new RecordAlreadyExistException($"Phase '{request.Name}' is already registered.");

        try
        {

            var phases = new Domain.Entities.FMS.FemPhase
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
               Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper(),
            };

            _context.FemPhases.Add(phases);
            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                phases.Id,
                "Phases registered successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Phases Creation failed: {ex.Message}", ex);
        }
    }
}



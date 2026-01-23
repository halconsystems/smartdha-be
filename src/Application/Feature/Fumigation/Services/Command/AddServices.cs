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

public record AddServicesCommand(
    [Required] string Name,
    [Required] string DisplayName
) : IRequest<SuccessResponse<Guid>>;
public class AddServicesCommandHandler
    : IRequestHandler<AddServicesCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public AddServicesCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddServicesCommand request, CancellationToken ct)
    {

        var existingPhase = await _context.FemServices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Name == request.Name &&
                    u.DisplayName == request.DisplayName,
                    ct);

        if (existingPhase != null)
            throw new RecordAlreadyExistException($"Phase '{request.Name}' is already registered.");

        try
        {

            var services = new Domain.Entities.FMS.FemService
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper(),
            };

            _context.FemServices.Add(services);
            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                services.Id,
                "Service registered successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Service Creation failed: {ex.Message}", ex);
        }
    }
}




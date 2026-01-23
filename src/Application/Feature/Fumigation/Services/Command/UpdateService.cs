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

public record UpdateServiceCommand(
    [Required] Guid Id,
    [Required] string Name,
    [Required] string DisplayName
) : IRequest<SuccessResponse<Guid>>;
public class UpdateServiceCommandHandler
    : IRequestHandler<UpdateServiceCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateServiceCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateServiceCommand request, CancellationToken ct)
    {

        var existingService = await _context.FemServices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingService == null)
            throw new RecordAlreadyExistException($"Service '{request.Name}' is Not registered.");

        try
        {
            existingService.Name = request.Name;
            existingService.DisplayName = request.DisplayName;
            existingService.Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper();

            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                existingService.Id,
                "Phases Updated successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Phases Updation failed: {ex.Message}", ex);
        }
    }
}




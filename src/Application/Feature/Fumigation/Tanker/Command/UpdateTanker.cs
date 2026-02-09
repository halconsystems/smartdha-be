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


public record UpdateTankerCommand(
    [Required] Guid Id,
    [Required] string Name,
    [Required] string DisplayName,
    [Required] string Price,
    string? ServiceId
) : IRequest<SuccessResponse<Guid>>;
public class UpdateTankerCommandHandler
    : IRequestHandler<UpdateTankerCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTankerCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateTankerCommand request, CancellationToken ct)
    {

        var existingTanker = await _context.TankerSizes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingTanker == null)
            throw new RecordAlreadyExistException($"Service '{request.Name}' is Not registered.");

        try
        {
            existingTanker.Name = request.Name;
            existingTanker.DisplayName = request.DisplayName;
            existingTanker.Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper();
            existingTanker.Price = request.Price;
            existingTanker.FemServiceId = string.IsNullOrEmpty(request.ServiceId) ? null : Guid.Parse(request.ServiceId);

            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                existingTanker.Id,
                "Tanker Updated successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Tanker Updation failed: {ex.Message}", ex);
        }
    }
}





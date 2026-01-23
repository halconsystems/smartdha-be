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


public record AddTankerCommand(
    [Required] string Name,
     [Required] string DisplayName,
     [Required] string Price
) : IRequest<SuccessResponse<Guid>>;
public class AddTankerCommandHandler
    : IRequestHandler<AddTankerCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public AddTankerCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddTankerCommand request, CancellationToken ct)
    {

        var existingTanker = await _context.TankerSizes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Name == request.Name &&
                    u.DisplayName == request.DisplayName,
                    ct);

        if (existingTanker != null)
            throw new RecordAlreadyExistException($"Tanker '{request.Name}' is already registered.");

        try
        {

            var tanker = new Domain.Entities.FMS.TankerSize
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper(),
                Price = request.Price,  
            };

            _context.TankerSizes.Add(tanker);
            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                tanker.Id,
                "Tanker registered successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Tanker Creation failed: {ex.Message}", ex);
        }
    }
}




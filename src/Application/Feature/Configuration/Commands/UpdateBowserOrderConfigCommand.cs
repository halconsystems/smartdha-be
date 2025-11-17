using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Configuration.Commands;

public record UpdateBowserOrderConfigCommand(bool Value)
    : IRequest<SuccessResponse<string>>;
public class UpdateBowserOrderConfigCommandHandler
    : IRequestHandler<UpdateBowserOrderConfigCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateBowserOrderConfigCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateBowserOrderConfigCommand request, CancellationToken cancellationToken)
    {
        // Get the specific configuration row with Key = BowserOrders
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == "BowserOrders", cancellationToken);

        if (config == null)
            throw new NotFoundException("Configuration", "BowserOrders");

        // Validate datatype
        if (!string.Equals(config.DataType, "bool", StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Configuration datatype is not boolean.");

        // Update value
        config.Value = request.Value.ToString().ToLower(); // "true"/"false"
        config.LastModified = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>("BowserOrders configuration updated successfully.");
    }
}

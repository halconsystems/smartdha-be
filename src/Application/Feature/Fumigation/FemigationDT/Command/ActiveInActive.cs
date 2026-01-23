using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;

internal class ActiveInActive
{
}

public record ActiveInActiveFMDTCommand(Guid Id, bool Active) : IRequest<SuccessResponse<string>>;
public class ActiveInActiveFMDTCommandHandler
    : IRequestHandler<ActiveInActiveFMDTCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public ActiveInActiveFMDTCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ActiveInActiveFMDTCommand command, CancellationToken ct)
    {
        var FemgutionShops = _context.FemDTSettings
            .FirstOrDefault(x => x.Id == command.Id);

        if (FemgutionShops is null) throw new KeyNotFoundException("Femgution discount/Tax Services not found.");

        FemgutionShops.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(FemgutionShops.Id.ToString());
    }
}




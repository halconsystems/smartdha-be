using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;

public record ActiveInActiveFMShopCommand(Guid Id, bool Active) : IRequest<SuccessResponse<string>>;
public class ActiveInActiveFMShopCommandCommandHandler
    : IRequestHandler<ActiveInActiveFMShopCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public ActiveInActiveFMShopCommandCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ActiveInActiveFMShopCommand command, CancellationToken ct)
    {
        var FemgutionShops = _context.FemgutionShops
            .FirstOrDefault(x => x.Id == command.Id);

        if (FemgutionShops is null) throw new KeyNotFoundException("Femgution Shops Services not found.");

        FemgutionShops.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(FemgutionShops.Id.ToString());
    }
}




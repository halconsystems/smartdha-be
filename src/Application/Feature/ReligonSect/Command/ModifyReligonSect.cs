using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ReligonSect.Command;

public record UpdateReligonSectCommand(Guid Id, string Name, string? DisplayName, Guid ReligonId) : IRequest<SuccessResponse<string>>;
public class UpdateReligonSectCommandHandler
    : IRequestHandler<UpdateReligonSectCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateReligonSectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateReligonSectCommand command, CancellationToken ct)
    {
        var ReligonSects = _context.ReligonSects
            .FirstOrDefault(x => x.Id == command.Id);

        if (ReligonSects is null) throw new KeyNotFoundException("Religon Section not found.");

        ReligonSects.Name = command.Name;
        ReligonSects.DisplayName = command.DisplayName;
        ReligonSects.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();
        ReligonSects.ReligonId = command.ReligonId;

        await _context.SaveChangesAsync(ct);
        return Success.Update(ReligonSects.Id.ToString());
    }
}


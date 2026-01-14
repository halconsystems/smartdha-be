using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Command;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Religion.Command;

public record UpdateReligionCommand(Guid Id, string name, string DisplayName)
    : IRequest<SuccessResponse<string>>;
public class UpdateReligionCommandHandler : IRequestHandler<UpdateReligionCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateReligionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateReligionCommand command, CancellationToken ct)
    {
        var religon = _context.Religions
            .FirstOrDefault(x => x.Id == command.Id);

        if (religon is null) throw new KeyNotFoundException("Religon not found.");

        religon.Name = command.name;
        religon.DisplayName = command.DisplayName;
        religon.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();

        await _context.SaveChangesAsync(ct);
        return Success.Update(religon.Id.ToString());
    }
}


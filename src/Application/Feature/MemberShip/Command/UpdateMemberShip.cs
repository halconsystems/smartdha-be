using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.MemberShip.Command;

public record UpdateMemberShipCommand(Guid Id, string Name, string? DisplayName) : IRequest<SuccessResponse<string>>;
public class UpdateMemberShipCommandHandler
    : IRequestHandler<UpdateMemberShipCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateMemberShipCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateMemberShipCommand command, CancellationToken ct)
    {
        var memberShip = _context.MemberShips
            .FirstOrDefault(x => x.Id == command.Id);

        if (memberShip is null) throw new KeyNotFoundException("MemberShip not found.");

        memberShip.Name = command.Name;
        memberShip.DisplayName = command.DisplayName;
        memberShip.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();

        await _context.SaveChangesAsync(ct);
        return Success.Update(memberShip.Id.ToString());
    }
}


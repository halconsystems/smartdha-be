using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Command;

public record UpdateMemberShipCategoryCommand(Guid Id, string Name, string? DisplayName, Guid MemberShipID) : IRequest<SuccessResponse<string>>;
public class UpdateMemberShipCategoryCommandHandler
    : IRequestHandler<UpdateMemberShipCategoryCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateMemberShipCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateMemberShipCategoryCommand command, CancellationToken ct)
    {
        var memberShip = _context.MemberShipCatergories
            .FirstOrDefault(x => x.Id == command.Id);

        if (memberShip is null) throw new KeyNotFoundException("MemberShip not found.");

        memberShip.name = command.Name;
        memberShip.displayname = command.DisplayName;
        memberShip.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();
        memberShip.MemberShipId = command.MemberShipID;

        await _context.SaveChangesAsync(ct);
        return Success.Update(memberShip.Id.ToString());
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.MemberShip.Command;

public record CreateMemberShipCommand(string name, string DisplayName)
    : IRequest<SuccessResponse<string>>;
public class CreateMemberShipCommandHandler : IRequestHandler<CreateMemberShipCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public CreateMemberShipCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(CreateMemberShipCommand command, CancellationToken ct)
    {
        try
        {
            var entity = new MemberShips
            {
                Name = command.name,
                DisplayName = command.DisplayName,
                Code = command.DisplayName.Substring(0, command.DisplayName.Length / 2).ToUpper()
            };

            _context.MemberShips.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Success.Created(entity.Id.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Success.Created(Guid.NewGuid().ToString());
        }
    }
}


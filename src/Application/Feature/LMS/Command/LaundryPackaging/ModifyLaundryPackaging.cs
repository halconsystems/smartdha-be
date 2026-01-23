using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;

public record ModifyLaundryPackagingCommand(Guid Id, string Name, string DisplayName, bool Active) : IRequest<SuccessResponse<string>>;
public class ModifyLaundryPackagingCommandHandler
    : IRequestHandler<ModifyLaundryPackagingCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ModifyLaundryPackagingCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ModifyLaundryPackagingCommand command, CancellationToken ct)
    {
        var LaundryPackagings = _context.LaundryPackagings
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryPackagings is null) throw new KeyNotFoundException("Laundry Packaging not found.");

        LaundryPackagings.Name = command.Name;
        LaundryPackagings.DisplayName = command.DisplayName;
        LaundryPackagings.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();
        LaundryPackagings.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryPackagings.Id.ToString());
    }
}




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;

public record ActiveInActiveLaundryPackagingCommand(Guid Id, bool Active) : IRequest<SuccessResponse<string>>;
public class ActiveInActiveLaundryPackagingCommandHandler
    : IRequestHandler<ActiveInActiveLaundryPackagingCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ActiveInActiveLaundryPackagingCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ActiveInActiveLaundryPackagingCommand command, CancellationToken ct)
    {
        var LaundryPackagings = _context.LaundryPackagings
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryPackagings is null) throw new KeyNotFoundException("Laundry Packaging not found.");

        LaundryPackagings.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryPackagings.Id.ToString());
    }
}





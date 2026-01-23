using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;

public record ActiveInActiveLaundryItemsCommand(Guid Id, bool Active) : IRequest<SuccessResponse<string>>;
public class ActiveInActiveLaundryItemsCommandHandler
    : IRequestHandler<ActiveInActiveLaundryItemsCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ActiveInActiveLaundryItemsCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ActiveInActiveLaundryItemsCommand command, CancellationToken ct)
    {
        var LaundryItems = _context.LaundryItems
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryItems is null) throw new KeyNotFoundException("Laundry item not found.");

        LaundryItems.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryItems.Id.ToString());
    }
}







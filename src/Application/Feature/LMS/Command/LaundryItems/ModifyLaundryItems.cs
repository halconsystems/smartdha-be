using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;

public record ModifyLaundryItemsCommand(Guid Id, string Name, string DisplayName, Guid CategoryID,string  ItemPrice) : IRequest<SuccessResponse<string>>;
public class ModifyLaundryItemsCommandHandler
    : IRequestHandler<ModifyLaundryItemsCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ModifyLaundryItemsCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ModifyLaundryItemsCommand command, CancellationToken ct)
    {
        var LaundryItems = _context.LaundryItems
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryItems is null) throw new KeyNotFoundException("Laundry item not found.");

        LaundryItems.Name = command.Name;
        LaundryItems.DisplayName = command.DisplayName;
        LaundryItems.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();
        LaundryItems.CategoryId = command.CategoryID;
        LaundryItems.ItemPrice = command.ItemPrice;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryItems.Id.ToString());
    }
}






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;

internal class DeleteLaundryItems
{
}

public record DeleteLaundryItemsCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteLaundryItemsCommandHandler
    : IRequestHandler<DeleteLaundryItemsCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public DeleteLaundryItemsCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteLaundryItemsCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.LaundryItems
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("Laundry Category not found.");

        return Success.Delete(command.Id.ToString());
    }

}






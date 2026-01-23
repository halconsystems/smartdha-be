using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryCategory;


public record ActiveInActiveLaundryCategoryCommand(Guid Id,bool Active) : IRequest<SuccessResponse<string>>;
public class ActiveInActiveLaundryCategoryCommandHandler
    : IRequestHandler<ActiveInActiveLaundryCategoryCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ActiveInActiveLaundryCategoryCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ActiveInActiveLaundryCategoryCommand command, CancellationToken ct)
    {
        var LaundryServices = _context.LaundryCategories
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryServices is null) throw new KeyNotFoundException("Laundry Services not found.");

        LaundryServices.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryServices.Id.ToString());
    }
}



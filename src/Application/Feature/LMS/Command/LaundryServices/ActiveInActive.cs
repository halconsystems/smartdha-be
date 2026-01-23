using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;



public record ActiveInActiveLaundryServiceCommand(Guid Id, bool Active) : IRequest<SuccessResponse<string>>;
public class ActiveInActiveLaundryServiceCommandHandler
    : IRequestHandler<ActiveInActiveLaundryServiceCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ActiveInActiveLaundryServiceCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ActiveInActiveLaundryServiceCommand command, CancellationToken ct)
    {
        var LaundryServices = _context.LaundryServices
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryServices is null) throw new KeyNotFoundException("Laundry Services not found.");

        LaundryServices.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryServices.Id.ToString());
    }
}



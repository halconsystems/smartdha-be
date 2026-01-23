using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;

public record ModifyLaundryServiceCommand(Guid Id, string Name, string DisplayName, bool Active) : IRequest<SuccessResponse<string>>;
public class ModifyLaundryServiceHandler
    : IRequestHandler<ModifyLaundryServiceCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ModifyLaundryServiceHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ModifyLaundryServiceCommand command, CancellationToken ct)
    {
        var LaundryServices = _context.LaundryServices
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryServices is null) throw new KeyNotFoundException("Laundry Services not found.");

        LaundryServices.Name = command.Name;
        LaundryServices.DisplayName = command.DisplayName;
        LaundryServices.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();
        LaundryServices.IsActive = command.Active;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryServices.Id.ToString());
    }
}



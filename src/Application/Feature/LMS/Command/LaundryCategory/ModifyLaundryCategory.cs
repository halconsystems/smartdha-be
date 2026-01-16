using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryCategory;

public record ModifyLaundryCategoryCommand(Guid Id, string Name, string DisplayName, Guid ReligonId) : IRequest<SuccessResponse<string>>;
public class ModifyLaundryCategoryCommandHandler
    : IRequestHandler<ModifyLaundryCategoryCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public ModifyLaundryCategoryCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(ModifyLaundryCategoryCommand command, CancellationToken ct)
    {
        var LaundryCategories = _context.LaundryCategories
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryCategories is null) throw new KeyNotFoundException("Laundry Categories not found.");

        LaundryCategories.Name = command.Name;
        LaundryCategories.DisplayName = command.DisplayName;
        LaundryCategories.Code = command.DisplayName?.Substring(0, command.DisplayName.Length / 2).ToUpper();

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryCategories.Id.ToString());
    }
}





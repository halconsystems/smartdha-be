using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Command;

internal class UpdateFemigationShopDT
{
}
public record UpdateFemigationShopDTCommand(Guid Id, Guid ShopId, Guid femDTId, string Value) : IRequest<SuccessResponse<string>>;
public class UpdateFemigationShopDTCommandHandler
    : IRequestHandler<UpdateFemigationShopDTCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateFemigationShopDTCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateFemigationShopDTCommand command, CancellationToken ct)
    {
        var LaundryServices = _context.ShopDTSettings
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryServices is null) throw new KeyNotFoundException("Shop Discount not found.");

        LaundryServices.ShopId = command.ShopId;
        LaundryServices.FemDTSettingId = command.femDTId;
        LaundryServices.Value = command.Value;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryServices.Id.ToString());
    }
}




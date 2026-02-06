using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryShopDT;

public record UpdateLaundryShopDTCommand(Guid Id, Guid ShopId, Guid OrderDTId, string Value) : IRequest<SuccessResponse<string>>;
public class UpdateLaundryShopDTCommandHandler
    : IRequestHandler<UpdateLaundryShopDTCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public UpdateLaundryShopDTCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(UpdateLaundryShopDTCommand command, CancellationToken ct)
    {
        var LaundryServices = _context.ShopDTSettings
            .FirstOrDefault(x => x.Id == command.Id);

        if (LaundryServices is null) throw new KeyNotFoundException("Shop Discount not found.");

        LaundryServices.ShopId = command.ShopId;
        LaundryServices.OrderDTSettingId = command.OrderDTId;
        LaundryServices.Value = command.Value;

        await _context.SaveChangesAsync(ct);
        return Success.Update(LaundryServices.Id.ToString());
    }
}



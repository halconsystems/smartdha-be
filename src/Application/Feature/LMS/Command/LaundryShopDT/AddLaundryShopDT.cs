using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryShopDT;
public record class CreateAddLaundryShopDTCommand : IRequest<SuccessResponse<string>>
{
    public Guid ShopId { get; set; }
    public Guid OrderDTSettingId { get; set; }
    [Required]
    public string Value { get; set; } = default!;
}

public class CreateAddLaundryShopDTCommandHandler
    : IRequestHandler<CreateAddLaundryShopDTCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateAddLaundryShopDTCommandHandler(ILaundrySystemDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(CreateAddLaundryShopDTCommand command, CancellationToken ct)
    {
        string UsedId = _currentUserService.UserId.ToString();


        var discount = new Domain.Entities.LMS.ShopDTSetting
        {
            ShopId = command.ShopId,
            OrderDTSettingId = command.OrderDTSettingId,
            Value = command.Value,

        };

        await _context.ShopDTSettings.AddAsync(discount, ct);
        await _context.SaveChangesAsync(ct);

        return Success.Created(discount.Id.ToString());
    }


}



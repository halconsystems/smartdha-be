using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Command;

public record class CreateOrderDiscountCommand : IRequest<SuccessResponse<string>>
{
    public Domain.Enums.Settings Name { get; set; }
    public Domain.Enums.ValueType ValueType { get; set; }
    public string? Value { get; set; }
    [Required]
    public string DisplayName { get; set; } = default!;
}

public class CreateOrderDiscountCommandHandler
    : IRequestHandler<CreateOrderDiscountCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOrderDiscountCommandHandler(ILaundrySystemDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(CreateOrderDiscountCommand command, CancellationToken ct)
    {
        string UsedId = _currentUserService.UserId.ToString();


        var discount = new Domain.Entities.LMS.OrderDTSetting
        {
            Name = command.Name,
            ValueType = command.ValueType,
            Value = command.Value,
            DisplayName = command.DisplayName,
            CreatedBy = UsedId,
            DTCode = command.Name.ToString().Substring(0, command.Name.ToString().Length / 2).ToUpper(),
            IsDiscount = SettingRules.IsDiscount(command.Name)

        };

        await _context.OrderDTSettings.AddAsync(discount, ct);
        await _context.SaveChangesAsync(ct);

        return Success.Created(discount.Id.ToString());
    }


}



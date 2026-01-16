using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;

public record class CreateDiscountCommand : IRequest<SuccessResponse<string>>
{

    public Guid UserId { get; set; }
    public Domain.Enums.Settings Name { get; set; }
    public Domain.Enums.ValueType ValueType { get; set; }
    public string? Value { get; set; }
    public string? DisplayName { get; set; }
}

public class CreateDiscountCommandHandler
    : IRequestHandler<CreateDiscountCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public CreateDiscountCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(CreateDiscountCommand command, CancellationToken ct)
    {


        var discount = new Domain.Entities.DiscountSetting
        {
            Name = command.Name,
            ValueType = command.ValueType,
            Value = command.Value,
            DisplayName = command.DisplayName,
            CreatedBy = command.UserId.ToString(),
            Code = command.Name.ToString().Substring(0, command.Name.ToString().Length / 2).ToUpper(),
            IsDiscount = SettingRules.IsDiscount(command.Name)

        };

        await _context.DiscountSettings.AddAsync(discount, ct);
        await _context.SaveChangesAsync(ct);

        return Success.Created(discount.Id.ToString());
    }


}


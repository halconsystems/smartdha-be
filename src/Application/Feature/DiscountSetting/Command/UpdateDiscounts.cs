using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;

public class UpdateDiscountsCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Domain.Enums.Settings Name { get; set; }
    public Domain.Enums.ValueType ValueType { get; set; }
    public string? Value { get; set; }
    public string? DisplayName { get; set; }
}

public class UpdateDiscountsCommandHandler
    : IRequestHandler<UpdateDiscountsCommand, bool>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateDiscountsCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateDiscountsCommand request, CancellationToken cancellationToken)
    {
        var discounts = await _context.DiscountSettings.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (discounts == null)
            return false;

        discounts.Name = request.Name;
        discounts.ValueType = request.ValueType;
        discounts.Value = request.Value;
        discounts.DisplayName = request.DisplayName;
        discounts.LastModifiedBy = request.UserId.ToString();
        discounts.Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper();
        discounts.IsDiscount = SettingRules.IsDiscount(request.Name);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

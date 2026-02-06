using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Command;


public record class CreateAddFemShopDTCommand : IRequest<SuccessResponse<string>>
{
    public Guid ShopId { get; set; }
    public Guid FemDTId { get; set; }
    [Required]
    public string Value { get; set; } = default!;
}

public class CreateAddFemShopDTCommandHandler
    : IRequestHandler<CreateAddFemShopDTCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateAddFemShopDTCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(CreateAddFemShopDTCommand command, CancellationToken ct)
    {
        string UsedId = _currentUserService.UserId.ToString();


        var discount = new Domain.Entities.FMS.ShopDTSetting
        {
            ShopId = command.ShopId,
            FemDTSettingId = command.FemDTId,
            Value = command.Value,

        };

        await _context.ShopDTSettings.AddAsync(discount, ct);
        await _context.SaveChangesAsync(ct);

        return Success.Created(discount.Id.ToString());
    }


}



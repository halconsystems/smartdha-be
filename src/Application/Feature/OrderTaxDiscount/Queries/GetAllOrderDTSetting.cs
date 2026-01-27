using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;


public record GetAllOrderDTSetting() : IRequest<List<OrderDTSettingDTO>>;

public class GetAllOrderDTSettingHandler : IRequestHandler<GetAllOrderDTSetting, List<OrderDTSettingDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly IProcedureService _sp;

    public GetAllOrderDTSettingHandler(ILaundrySystemDbContext userManager, IProcedureService sp    )
    {
        _context = userManager;
        _sp = sp;
    }

    public async Task<List<OrderDTSettingDTO>> Handle(GetAllOrderDTSetting request, CancellationToken ct)
    {
        var drivers = await _context.OrderDTSettings
            .Where(x => x.Name != Settings.Hanger)
            .AsNoTracking()
            .Select(x => new OrderDTSettingDTO
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value,
                ValueType = x.ValueType,
                IsDiscount = x.IsDiscount,
                DisplayName = x.DisplayName,
                DTCode = x.DTCode
            })
            .ToListAsync(ct);


        return drivers;
    }
}




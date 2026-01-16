using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;


public record HangerPriceAdjustmentQuery(Guid PackageId) : IRequest<List<LaundryItemsDTO>>;
public class HangerPriceAdjustmentQueryHandler : IRequestHandler<HangerPriceAdjustmentQuery, List<LaundryItemsDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public HangerPriceAdjustmentQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<LaundryItemsDTO>> Handle(HangerPriceAdjustmentQuery request, CancellationToken ct)
    {
        var package = await _context.LaundryPackagings.FirstOrDefaultAsync(x => x.Id == request.PackageId);

        if(package == null )
            throw new KeyNotFoundException("Package not found.");
        if (package.Code == "HAN")
        {
            decimal increment = 0;
            var increasePrice = await _context.OrderDTSettings.Where(x => x.DTCode == "HAN").FirstOrDefaultAsync();
            if(increasePrice == null)
            {
                throw new KeyNotFoundException("Price Increase not found.");
            }
            increment = increasePrice.ValueType == Domain.Enums.ValueType.Percent ? (Convert.ToDecimal(increasePrice.Value) / 100) : (Convert.ToDecimal(increasePrice.Value));
            var LaundryItems = await _context.LaundryItems
            .AsNoTracking()
            .ToListAsync(ct);

            var result = LaundryItems.Select(x => new LaundryItemsDTO
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                Code = x.Code,
                CategoryID = x.CategoryId.ToString(),
                ItemPrice = ((Convert.ToDecimal(x.ItemPrice)) + increment).ToString(),
            }).ToList();

            return result;
        }
        else
        {
            var LaundryItems = await _context.LaundryItems
           .AsNoTracking()
           .ToListAsync(ct);

            var result = LaundryItems.Select(x => new LaundryItemsDTO
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                Code = x.Code,
                CategoryID = x.CategoryId.ToString(),
                ItemPrice = x.ItemPrice,
            }).ToList();

            return result;

        }


           
    }
}






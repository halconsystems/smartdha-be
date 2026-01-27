using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;


public record HangerPriceAdjustmentQuery(Guid PackageId) : IRequest<List<LaundryItemsDTO>>;
public class HangerPriceAdjustmentQueryHandler : IRequestHandler<HangerPriceAdjustmentQuery, List<LaundryItemsDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    public HangerPriceAdjustmentQueryHandler(ILaundrySystemDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<LaundryItemsDTO>> Handle(HangerPriceAdjustmentQuery request, CancellationToken ct)
    {
        var package = await _context.LaundryPackagings.FirstOrDefaultAsync(x => x.Id == request.PackageId);
        var laundryCategory = await _context.LaundryCategories.FirstOrDefaultAsync(x => x.Code == "HOME");

        if (laundryCategory == null)
            laundryCategory = new Domain.Entities.LaundryCategory();

        if (package == null )
            throw new KeyNotFoundException("Package not found.");
        if (package.Code == "HANGE")
        {
            decimal increment = 0;
            var increasePrice = await _context.OrderDTSettings.Where(x => x.Name == Settings.Hanger).FirstOrDefaultAsync();
            if(increasePrice == null)
            {
                throw new KeyNotFoundException("Price Increase not found.");
            }
            increment = increasePrice.ValueType == Domain.Enums.ValueType.Percent ? (Convert.ToDecimal(increasePrice.Value) / 100) : (Convert.ToDecimal(increasePrice.Value));
            var LaundryItems = await _context.LaundryItems
            .AsNoTracking()
            .ToListAsync(ct);

            var result = LaundryItems.Select(x =>
            {
                decimal basePrice = Convert.ToDecimal(x.ItemPrice);

                // Increase price only if NOT HOME category
                decimal finalPrice = x.CategoryId == laundryCategory.Id
                    ? basePrice
                    : basePrice + (increasePrice.ValueType == Domain.Enums.ValueType.Percent
                        ? basePrice * increment
                        : increment);

                return new LaundryItemsDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Code = x.Code,
                    CategoryID = x.CategoryId.ToString(),
                    ItemPrice = finalPrice.ToString(),
                    Image = x.ItemImage == null ? null : _fileStorageService.GetPublicUrl(x.ItemImage)
                };
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
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName,
                Code = x.Code,
                CategoryID = x.CategoryId.ToString(),
                ItemPrice = x.ItemPrice,
                Image = x.ItemImage == null ? null : _fileStorageService.GetPublicUrl(x.ItemImage)
            }).ToList();

            return result;

        }


           
    }
}






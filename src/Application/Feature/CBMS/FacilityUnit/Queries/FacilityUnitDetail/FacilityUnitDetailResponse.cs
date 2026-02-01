using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries.FacilityUnitDetail;
public record FacilityUnitDetailResponse(
    Guid FacilityUnitId,
    string UnitName,
    string? UnitType,

    FacilityDto Facility,
    List<ImageDto> Images,
    List<FacilityUnitServiceDto> Services,

    List<PriceModifierDto> Discounts,
    List<PriceModifierDto> Taxes
);
public record PriceModifierDto(
    Guid Id,
    string Name,
    string Code,
    DiscountOrTaxType Type,
    decimal Value,
    decimal CalculatedAmount
);

public enum DiscountOrTaxType
{
    Percentage = 1,
    Flat = 2
}

public record FacilityDto(
    Guid FacilityId,
    string Name,
    string DisplayName,
    string Code,
    string? Description
);

public record FacilityUnitServiceDto(
    Guid FacilityServiceId,
    string Name,
    string Code,
    bool IsComplimentary,
    bool IsQuantityBased,
    decimal Price,
    bool IsEnabled
);

public record ImageDto(
    string ImageURL,
    string ImageExtension,
    string? ImageName,
    string? Description,
    ImageCategory Category
);


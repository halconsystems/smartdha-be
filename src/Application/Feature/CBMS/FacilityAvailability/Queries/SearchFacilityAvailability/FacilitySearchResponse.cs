using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityAvailability.Queries.SearchFacilityAvailability;
public record FacilitySearchResponse(
    Guid FacilityId,
    string FacilityName,
    string CategoryName,
    string? ImageUrl,
    BookingMode BookingMode,
    List<FacilityUnitAvailabilityDto> Units
);

public record FacilityUnitAvailabilityDto(
    Guid FacilityUnitId,
    string UnitName,
    string? UnitType,
    decimal BasePrice,
    bool IsAvailable,
    List<SlotAvailabilityDto>? Slots,
    string? MainImageUrl
);

public record SlotAvailabilityDto(
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal Price,
    bool IsAvailable
);

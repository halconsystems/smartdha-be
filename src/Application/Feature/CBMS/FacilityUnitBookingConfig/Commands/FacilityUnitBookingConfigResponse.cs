using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands;
public record FacilityUnitBookingConfigResponse(
    Guid Id,
    Guid FacilityUnitId,
    BookingMode BookingMode,
    bool RequiresApproval,
    int? SlotDurationMinutes,
    TimeOnly? OpeningTime,
    TimeOnly? ClosingTime,
    decimal BasePrice,
    int MaxConcurrentBookings,
    bool UseAvailabilityRules
);


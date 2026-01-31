using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.CreateFacilityUnitBookingConfig;
public record CreateFacilityUnitBookingConfigDto(
    Guid FacilityUnitId,
    BookingMode BookingMode,
    decimal BasePrice,
    bool RequiresApproval,
    int? SlotDurationMinutes,
    TimeOnly? OpeningTime,
    TimeOnly? ClosingTime
);


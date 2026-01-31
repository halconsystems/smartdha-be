using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.AddFacilityUnitService;
public record AddFacilityUnitServiceDto(
    Guid FacilityUnitId,
    Guid FacilityServiceId,
    decimal? OverridePrice,
    bool IsEnabled
);


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.CreateFacilityService;
public record CreateFacilityServiceDto(
    Guid FacilityId,
    string Name,
    string Code,
    decimal Price,
    bool IsComplimentary,
    bool IsQuantityBased
);


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Commands.CreateFacilityUnit;
public record CreateFacilityUnitDto(
    Guid ClubId,
    Guid FacilityId,
    string Name,
    string Code,
    string? UnitType
);


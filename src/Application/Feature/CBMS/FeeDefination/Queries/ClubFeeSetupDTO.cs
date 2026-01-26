using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FeeDefination.Queries;


public record ClubFeeSlabDto(
    decimal FromArea,
    decimal ToArea,
    decimal Amount
);

public record ClubFeeSetupDTO(
    Guid FeeDefinitionId,
    FeeType FeeType,
    decimal? FixedAmount,
    AreaUnit? AreaUnit,
    bool AllowOverride,
    List<ClubFeeSlabDto> Slabs
);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Queries.GetFeeSettings;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries;
public record FeeDefinitionDto(
    Guid FeeDefinitionId,
    FeeType FeeType,
    decimal? FixedAmount,
    AreaUnit? AreaUnit,
    bool AllowOverride,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? Notes,

    // Option-based response
    List<FeeCategoryDto>? Categories, // for OptionBasedWithCategory
    List<FeeOptionDto>? Options,        // for OptionBased
    List<FeeSettingDto> ExtraCharges
);


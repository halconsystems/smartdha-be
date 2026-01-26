using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition;
public record ProcessPrerequisiteDto
(
    Guid Id,
    Guid ProcessId,
    Guid PrerequisiteDefinitionId,
    string Code,
    string Name,
    PrerequisiteType Type,

    bool IsRequired,
    int RequiredByStepNo,

    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    List<PrerequisiteOptionDto> Options
);

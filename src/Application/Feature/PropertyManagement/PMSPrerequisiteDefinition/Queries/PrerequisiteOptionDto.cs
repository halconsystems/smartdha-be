using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries;
public record PrerequisiteOptionDto
(
    Guid Id,
    string Label,
    string Value,
    int SortOrder
);

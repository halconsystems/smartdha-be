using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands;
public record PrerequisiteOptionInput(
    string Label,
    string Value,
    int SortOrder = 0
);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries;
public record ResidenceTypeDto(
    Guid Id,
    string Name,
    string? Description
);

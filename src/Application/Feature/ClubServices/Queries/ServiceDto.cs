using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Queries;
public record ServiceDto(
    Guid Id,
    string Name,
    string? Description,
    bool? IsActive,
    bool? IsDeleted
);


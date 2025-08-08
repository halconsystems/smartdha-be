using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries;
public record RoomCategoryDto(
    Guid Id,
    string Name,
    string? Description
);


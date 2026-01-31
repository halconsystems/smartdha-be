using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries;
public record IdClubDto(
    Guid Id,
    string Name,
    string? Description,
    string? Location,
    string? ContactNumber,
    string? AccountNo,
    string? AccountNoAccronym,
    bool? IsActive,
    bool? IsDeleted,
    string? Email
);

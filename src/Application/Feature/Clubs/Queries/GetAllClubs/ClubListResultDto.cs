using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetAllClubs;
public class ClubListResultDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; }
    public string? Location { get; set; } = default!;
    public string? ContactNumber { get; set; } = default!;

    public ClubMainImageDto? MainImage { get; set; }
}
public class ClubMainImageDto
{
    public Guid ImageId { get; set; }
    public string ImageUrl { get; set; } = default!;
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
public class FacilityDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ImageUrl { get; set; }

    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; }

    public bool HasAction { get; set; }
    public string? ActionName { get; set; }
    public string? ActionType { get; set; }
}


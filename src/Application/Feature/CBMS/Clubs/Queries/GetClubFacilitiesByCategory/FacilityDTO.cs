using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
public class FacilityDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ImageUrl { get; set; }

    public BookingMode BookingMode { get; set; } = BookingMode.None;
    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsPriceVisible { get; set; } = true;
    public bool HasAction { get; set; }
    public FacilityActionType ActionTypeEnum { get; set; } = FacilityActionType.None;
    public string? DisplayName { get; set; }
}


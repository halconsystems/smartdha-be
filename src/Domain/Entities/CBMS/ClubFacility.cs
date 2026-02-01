using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class ClubFacility : BaseAuditableEntity
{
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;
    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;
    public BookingMode BookingMode { get; set; } = BookingMode.None;
    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsPriceVisible { get; set; } = true;
    public bool HasAction { get; set; }
    public string? ActionName { get; set; }
    public string? ActionType { get; set; }
    public FacilityActionType FacilityActionType { get; set; } = FacilityActionType.None;
    public FacilityActionType ActionTypeEnum
    {
        get
        {
            return ActionType switch
            {
                "Book Now" => FacilityActionType.Book,
                "Reserve" => FacilityActionType.Reserve,
                "Contact Us" => FacilityActionType.ContactUs,
                "View Detail" => FacilityActionType.ViewDetails,
                _ => FacilityActionType.None,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BanquetBooking : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = default!;

    public int ExpectedGuests { get; set; }

    public FoodType? FoodType { get; set; }

    public bool IsDecorationRequired { get; set; }
    public bool IsSoundSystemRequired { get; set; }

    public string? SpecialInstructions { get; set; }
}


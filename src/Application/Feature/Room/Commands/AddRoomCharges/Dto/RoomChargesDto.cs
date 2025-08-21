using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;

public class RoomChargesDto
{
    public Guid RoomId { get; set; }
    public RoomBookingType BookingType { get; set; } = RoomBookingType.Self;
    public int NoOfOccupancy { get; set; }
    public decimal Charges { get; set; }
}

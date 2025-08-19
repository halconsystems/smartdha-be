using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;

public class RoomChargesDto
{
    public string BookingType { get; set; } = default!;
    public decimal Charges { get; set; }
}

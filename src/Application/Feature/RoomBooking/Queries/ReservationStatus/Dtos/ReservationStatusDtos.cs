using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
public class ReservationStatusDto
{
    public StageStatus? ReservationStage { get; set; }
    public StageStatus? PaymentStage { get; set; }
    public StageStatus? BookingStage { get; set; }
}

public class StageStatus
{
    public string? Status { get; set; }
    public int? StatusCode { get; set; }

    public static implicit operator StageStatus(Reservationstage v)
    {
        throw new NotImplementedException();
    }
}


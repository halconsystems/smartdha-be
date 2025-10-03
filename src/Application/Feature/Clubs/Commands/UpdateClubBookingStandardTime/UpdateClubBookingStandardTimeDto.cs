using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
public record UpdateClubBookingStandardTimeDto(
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime
);

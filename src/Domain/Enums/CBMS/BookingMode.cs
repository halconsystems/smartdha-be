using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.CBMS;
public enum BookingMode
{
    DayBased = 1,      // Banquet, Grounds
    SlotBased = 2,     // Padel, Tennis
    Hourly = 3,        // Grounds, Halls
    NightBased = 4,    // Guest Rooms
    BookingMode= 5      //Guest Rooms/Banquet
}


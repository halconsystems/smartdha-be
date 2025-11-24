using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum SvVehicleType
{
    Motorcycle = 1,
    Ambulance = 2,
    FireBrigade = 3,
    SecurityVan = 4,
    Other = 99
}

// Domain/Enums/SvVehicleStatus.cs
public enum SvVehicleStatus
{
    Offline = 0,
    Available = 1,
    Busy = 2,
    Maintenance = 3
}

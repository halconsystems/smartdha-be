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

public enum ShopVehicleType
{
    Motorcycle = 1,
    Rikshaw = 2,
    Car = 3,
    Truck = 4,
    Other = 99
}

public enum ShopVehicleStatus
{
    Offline = 0,
    Available = 1,
    Busy = 2,
    Maintenance = 3
}

public enum FemugationVehicleType
{
    Motorcycle = 1,
    Rikshaw = 2,
    Car = 3,
    Truck = 4,
    Other = 99
}

public enum FMVehicleStatus
{
    Offline = 0,
    Available = 1,
    Busy = 2,
    Maintenance = 3
}

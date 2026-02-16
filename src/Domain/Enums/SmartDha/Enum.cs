using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum CategoryType
{
    Residential = 1,
    Commercial = 1,
}
public enum Zone
{
    ZoneA = 1,
    ZoneB = 2,
    ZoneC = 3,
    ZoneD = 4,
    ZoneE = 5,
    ZoneF = 6,
}
public enum RelationUserFamily
{
    Father = 1,
    Mother = 2,
    Son = 3,
    Daughter = 4,
    Brother = 5,
    Sister = 6,
    Spouse = 7,
}

public enum ResidenceStatusDha
{
    Owner = 1,
    Tentent = 2,
}
public enum Phase
{
    Phase1 = 1,
    Phase2 = 2,
    Phase3 = 3,
    Phase4 = 4,
    Phase5 = 5,
    Phase6 = 6,
    Phase7 = 7,
    Phase8 = 8
}

public enum PropertyTypeCommercial { 

   Shop = 1,
   Office = 2,
   Building = 3,
}
public enum PropertyTypeResidantial
{
    Shop = 1,
    Office = 2,
    Building = 3,
}
public enum JobType
{
    Driver = 1,
    Cook = 2,
    Guard = 3,
    Peon = 4,
    Gardener = 5,
}
public enum WorkerCardDeliveryType
{
    OwnerOrEmployeerAddress = 1,
    SelfPickUp = 2,
}
public enum QuickPickType
{
    DayPass = 1,
    LongDay = 2,
}


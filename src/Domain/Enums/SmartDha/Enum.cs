using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum CategoryType
{
    Residential = 1,
    Commercial = 2,
}
public enum MemberType
{
    Residential = 1,
    Commercial = 2,
    EducationalVisitor = 3,
    CommercialEmployee = 4,
    HouseHelp = 5,
    Visitor = 6,
    Other = 7,
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
public enum PropertyTypeResidential
{
    Bunglow = 1,
    Flat = 2,
    Portion = 3,
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
public enum VisitorPassType
{
    DayPass = 1,
    LongDay = 2,
}
public enum EnumType
{
    CategoryType = 1,
    MemberType = 2,
    Zone = 3,
    RelationUserFamily = 4,
    ResidenceStatusDha = 5,
    Phase = 6,
    PropertyTypeCommercial = 7,
    PropertyTypeResidential = 8,
    JobType = 9,
    WorkerCardDeliveryType = 10,
    VisitorPassType = 11
}

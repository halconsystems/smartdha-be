using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;

public enum MaritalStatus
{
    Single = 1,
    Married = 2,
    Divorce = 3,
    Widow = 4,
    Widower = 5,
}


public enum OgranizationType
{
    Business = 0,
    NGO = 1,
    Diplomat = 2,
    Trust = 3,
    Other = 99
}

public enum CompanyType
{
    Limited = 0,
    Unlimited = 1,
    PatnerSHip = 2
}

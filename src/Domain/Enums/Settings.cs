using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;

public enum Settings
{
    Member = 1,
    MemStaff = 2,
    NonMember = 3,
    Employee = 4,
    GST = 5,
    FED = 6,
    AdvanceTax = 7,
    Shipping = 8,
    Hanger = 9,
    Academy = 10,
}

public enum ValueType
{
    Percent = 1,
    Decimal = 2,
    Currency = 3,
    Fraction = 4,
    Ratio = 5,
    Rate = 6,
    Score = 7,
}

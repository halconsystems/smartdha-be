using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public enum FeeType
{
    Fixed = 0,
    AreaBased = 1,
    Manual = 2, // Finance enters amount at runtime
    OptionBased = 3,              // Ordinary / Urgent
    OptionBasedWithCategory = 4   // Category A/B + Ordinary/Urgent
}

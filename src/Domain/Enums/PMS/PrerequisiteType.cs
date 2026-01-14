using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public enum PrerequisiteType
{
    Text = 0,
    Number = 1,
    Date = 2,
    Boolean = 3,
    File = 4,             // CNIC upload, drawings, etc.
    ExternalVerification = 5 // NADRA verification, etc.
}

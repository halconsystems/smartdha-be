using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum PanicDispatchStatus
{
    Assigned = 1,   // Control room assigned
    Accepted = 2,   // Driver accepted
    Arrived = 4,    // Reached location
    Completed = 5,  // Action completed
    Cancelled = 6
}


public enum FemugationDispatchStatus
{
    Assigned = 1,   // Control room assigned
    Accepted = 2,   // Driver accepted
    Arrived = 4,    // Reached location
    Completed = 5,  // Action completed
    Cancelled = 6
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public enum StepAction
{
    Submit = 0,
    Forward = 1,
    Return = 2,
    Reject = 3,
    Approve = 4,
    Hold = 5,
    Resume = 6
}

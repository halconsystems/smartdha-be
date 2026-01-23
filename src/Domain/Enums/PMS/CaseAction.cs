using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public enum CaseAction
{
    Received,
    Assigned,
    Claimed,
    CompletedInternal,
    ForwardExternal,
    Returned,
    Rejected,
    Approved,
    ForwardInternal
}


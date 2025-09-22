using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum PanicActionType
{
    Create = 1,
    Acknowledge = 2,
    AssignResponder = 3,
    UpdateStatus = 4,
    UpdateLocation = 5,
    AddNote = 6,
    Resolve = 7,
    Cancel = 8
}

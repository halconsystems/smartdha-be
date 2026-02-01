using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.CBMS;
public enum FacilityActionType
{
    None = 0,
    Book = 1,          // Booking flow
    ViewDetails = 2,   // Details screen
    ContactUs= 3,    // Contact us flow
    Reserve = 4        // Special reservation flow
}


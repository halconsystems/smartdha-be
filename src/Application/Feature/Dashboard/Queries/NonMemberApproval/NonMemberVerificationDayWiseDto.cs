using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.NonMemberApproval;
public record NonMemberVerificationDayWiseDto(
    string Date,   // "yyyy-MM-dd" for easy charting
    int Requests,
    int Approved,
    int Rejected
);

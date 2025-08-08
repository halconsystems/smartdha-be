using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.NonMemberApproval;
public record NonMemberVerificationSummaryDto(
    int TotalRequests,
    int Approved,
    int Rejected,
    int Pending
);

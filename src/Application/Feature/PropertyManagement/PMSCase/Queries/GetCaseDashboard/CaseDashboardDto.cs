using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseDashboard;
public class CaseDashboardDto
{
    public CaseDashboardSummary Overall { get; set; } = new();
    public CaseDashboardSummary Today { get; set; } = new();
    public CaseDashboardSummary ThisWeek { get; set; } = new();
    public CaseDashboardSummary ThisMonth { get; set; } = new();
    // PAYMENT STATS
    public CasePaymentDashboardSummary Payments { get; set; } = new();
}
public class CaseDashboardSummary
{
    public int Total { get; set; }
    public int Draft { get; set; }
    public int Submitted { get; set; }
    public int InProgress { get; set; }
    public int Approved { get; set; }
    public int Returned { get; set; }
    public int Rejected { get; set; }
}
public class CasePaymentDashboardSummary
{
    public int TotalCasesWithFee { get; set; }

    public int Paid { get; set; }
    public int Pending { get; set; }
    public int Failed { get; set; }

    public decimal TotalPayable { get; set; }
    public decimal TotalPaid { get; set; }
}


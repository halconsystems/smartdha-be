using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web;
public class ComplaintDashboardSummaryDto
{
    public int TotalComplaints { get; set; }

    public Dictionary<string, int> ComplaintsByStatus { get; set; } = new();
    public Dictionary<string, int> ComplaintsByCategory { get; set; } = new();
    public Dictionary<string, int> ComplaintsByPriority { get; set; } = new();

    public List<RecentComplaintDto> RecentComplaints { get; set; } = new();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaintStatusCount;
public class ComplaintStatusCountVm
{
    public int TotalComplaints { get; set; }
    public int New { get; set; }
    public int Acknowledged { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Reopened { get; set; }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Queries.GetMyComplaints;
public class MyComplaintVm
{
    public Guid Id { get; set; }
    public string ComplaintNo { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Notes { get; set; } = default!;
    public string CategoryCode { get; set; } = default!;
    public string PriorityCode { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime Created { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}


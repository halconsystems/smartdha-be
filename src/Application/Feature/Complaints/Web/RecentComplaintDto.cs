using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web;
public class RecentComplaintDto
{
    public Guid Id { get; set; }
    public string ComplaintNo { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public DateTime Created { get; set; }
}

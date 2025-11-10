using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ComplaintComment : BaseAuditableEntity
{
    public int ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = default!;
    public string Text { get; set; } = default!;
    public ComplaintVisibility Visibility { get; set; } = ComplaintVisibility.Public;
    public string CreatedByUserId { get; set; } = default!;
}


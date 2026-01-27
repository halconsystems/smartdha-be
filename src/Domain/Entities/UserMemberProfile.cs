using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserMemberProfile : BaseAuditableEntity
{
    //FK to Identity User
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    //SP OUTPUT DATA
    public string? MemId { get; set; }
    public string? MemNo { get; set; }
    public string? StaffNo { get; set; }
    public string? Category { get; set; }
    public string? Name { get; set; }
    public string? ApplicationDate { get; set; }
    public string? NIC { get; set; }
    public string? CellNo { get; set; }
    public string? AllReplot { get; set; }
    public string? MemPk { get; set; }
    public string? Email { get; set; }
    public string? DOB { get; set; }

    public string? Message { get; set; }

    // 🔍 Useful flags
    public bool IsMember => !string.IsNullOrEmpty(MemNo);
    public bool IsEmployee => !string.IsNullOrEmpty(StaffNo);
}


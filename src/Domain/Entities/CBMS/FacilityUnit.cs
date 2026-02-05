using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnit : BaseAuditableEntity
{
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;
    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;

    public string Name { get; set; } = default!;   // Room 101, Padel Court A
    public string Code { get; set; } = default!;   // R-101, PC-A
    public string? Description { get; set; }
    public string? UnitType { get; set; }          // Premium, Standard, VIP

    // 🔴 THIS WAS MISSING
    public ICollection<FacilityUnitService> FacilityUnitServices { get; set; }
        = new List<FacilityUnitService>();

    public ICollection<FacilityUnitImage> FacilityUnitImages { get; set; } = new List<FacilityUnitImage>();

}


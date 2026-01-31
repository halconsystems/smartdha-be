using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityAvailability : BaseAuditableEntity
{
    public Guid FacilityId { get; set; }

    public DateOnly Date { get; set; }

    public Guid? SlotId { get; set; } // null for day-based

    public bool IsAvailable { get; set; } = true;
}


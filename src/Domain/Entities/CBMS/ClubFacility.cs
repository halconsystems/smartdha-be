using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class ClubFacility : BaseAuditableEntity
{
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;

    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;

    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsPriceVisible { get; set; } = true;

    // UI / Action behavior
    public bool HasAction { get; set; }
    public string? ActionName { get; set; }   // Book / Order / Reserve
    public string? ActionType { get; set; }   // API / URL / Modal
}


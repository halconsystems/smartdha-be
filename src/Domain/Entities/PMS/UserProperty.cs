using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class UserProperty : BaseAuditableEntity
{
    // DHA reference keys
    [Required, MaxLength(50)]
    public string PropertyNo { get; set; } = default!;   // PLOT_NO / PLTNO

    [MaxLength(100)]
    public string? Sector { get; set; }                  // STNAME / SUBDIV

    [MaxLength(50)]
    public string? Phase { get; set; }                   // PHASE

    [MaxLength(20)]
    public string? OwnerCnic { get; set; }               // NIC

    [MaxLength(50)]
    public string PlotNo { get; set; } = default!;       // PLOT_NO

    public string? Area { get; set; }                   // ACTUAL_SIZE

    // DHA internal keys (optional but recommended)
    public string? PropertyPk { get; set; }                // PLOTPK
    public string? MemberPk { get; set; }                  // MEMPK

    [MaxLength(20)]
    public string? MemberNo { get; set; }                // MEMNO

    [MaxLength(20)]
    public string? CellNo { get; set; }                  // CELLNO
}


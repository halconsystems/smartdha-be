using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoomExtraOccupancyCharge : BaseAuditableEntity
{
    [Required]
    public Guid RoomChargeID { get; set; }
    public RoomCharge RoomCharge { get; set; } = default!;

    [Required]
    public int MaxOccupancy { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Charges { get; set; }
}

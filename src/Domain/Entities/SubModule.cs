using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class SubModule : BaseAuditableEntity
{
    [Required, MaxLength(100)]
    public string Value { get; set; } = default!;         // e.g. "Club.RoomReservation"
    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = default!;   // e.g. "Room Reservation"

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(100)]
    public string Description { get; set; } = default!;
    public Guid? ModuleId { get; set; }
    public Module Module { get; set; } = default!;
    public bool RequiresPermission { get; set; } = false;

    public ICollection<AppPermission> Permissions { get; set; } = new List<AppPermission>();
}


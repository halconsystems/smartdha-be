using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicDispatch : BaseAuditableEntity
{
    public Guid PanicId { get; set; }
    public PanicRequest PanicRequest { get; set; } = default!; // existing entity

    public Guid SvVehicleId { get; set; }
    public SvVehicle SvVehicle { get; set; } = default!;

    public PanicDispatchStatus Status { get; set; } = PanicDispatchStatus.Assigned;

    public DateTimeOffset AssignedAtUtc { get; set; }
    public string AssignedByUserId { get; set; } = default!;  // Control room user
    // public ApplicationUser AssignedByUser { get; set; }
    public DateTimeOffset? AcceptedAtUtc { get; set; }
    public DateTimeOffset? OnRouteAtUtc { get; set; }
    public DateTimeOffset? ArrivedAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public DateTimeOffset? CancelledAtUtc { get; set; }
    public string? ControlRoomRemarks { get; set; }
    public string? DriverRemarks { get; set; }
}

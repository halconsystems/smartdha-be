using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicDispatch : BaseAuditableEntity
{
    public Guid PanicRequestId { get; set; }
    public PanicRequest PanicRequest { get; set; } = default!; // existing entity
    public Guid SvVehicleId { get; set; }
    public SvVehicle SvVehicle { get; set; } = default!;
    public PanicDispatchStatus Status { get; set; } = PanicDispatchStatus.Assigned;
    public DateTime AssignedAt { get; set; }
    public string AssignedByUserId { get; set; } = default!;  // Control room user
    // public ApplicationUser AssignedByUser { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? ControlRoomRemarks { get; set; }
    public string? DriverRemarks { get; set; }
    //VOICE remarks (optional, mp3)
    public string? DriverRemarksAudioPath { get; set; }   // relative path

    // geo
    public double? AcceptedAtLatitude { get; set; }
    public double? AcceptedAtLongitude { get; set; }
    [MaxLength(300)] public string? AcceptedAtAddress { get; set; }
    public double? DistanceFromPanicKm { get; set; }
    public DateTime? LastLocationUpdateAt { get; set; }
    public string? DriverUserId { get; set; }    // nullable
    //Add navigation in PanicDispatch (optional but useful):
    public ICollection<PanicDispatchMedia> Media { get; set; } = new List<PanicDispatchMedia>();

}

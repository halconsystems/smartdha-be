using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Domain.Entities;
// Domain/Entities/SvVehicle.cs
public class SvVehicle : BaseAuditableEntity
{
    public string Name { get; set; } = default!;          // "Bike-01", "Ambulance-02"
    public string RegistrationNo { get; set; } = default!; // vehicle number
    public SvVehicleType VehicleType { get; set; }
    // For map icon (front-end will map this to specific image)
    public string? MapIconKey { get; set; }  // e.g. "bike", "ambulance", "firetruck"
    // Assigned point (where this vehicle is normally parked)
    public Guid? SvPointId { get; set; }      // nullable
    public SvPoint? SvPoint { get; set; }     // nullable navigation
                                              // OPTIONAL: which user (driver) owns this vehicle in the mobile app
    public string? DriverUserId { get; set; }   // nullable
    public ApplicationUser? Driver { get; set; }  // nullable navigation


    // Live tracking (optional, from driver app)
    public double? LastLatitude { get; set; }   
    public double? LastLongitude { get; set; }  
    public DateTime? LastLocationAt { get; set; }
    public SvVehicleStatus Status { get; set; } = SvVehicleStatus.Offline;
    public ICollection<OrderDispatch> OrderDispatches { get; set; } = new List<OrderDispatch>();
}


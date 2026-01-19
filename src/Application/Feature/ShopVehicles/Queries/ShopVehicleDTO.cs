using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ShopVehicles.Queries;

public class ShopVehicleDTO
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public Domain.Entities.LMS.Shops? Shops { get; set; }
    public string Name { get; set; } = default!;          // "Bike-01", "Ambulance-02"
    public string RegistrationNo { get; set; } = default!; // vehicle number
    public ShopVehicleType VehicleType { get; set; }
    // For map icon (front-end will map this to specific image)
    public string? MapIconKey { get; set; }  // e.g. "bike", "ambulance", "firetruck"
    public Guid? DriverUserId { get; set; }   // nullable


    // Live tracking (optional, from driver app)
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationAt { get; set; }
    public ShopVehicleStatus Status { get; set; } = ShopVehicleStatus.Offline;

    public List<Application.Feature.Shops.Queries.ShopQueryDTO>? ShopQuery { get; set; }

}

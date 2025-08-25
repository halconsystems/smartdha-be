using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserRequest : BaseAuditableEntity
{
    public string RequestNo { get; set; } = default!;

    public DateTime RequestDate { get; set; }

    //public Guid RequestFor { get; set; }

    public DateTime RequestedDeliveryDate { get; set; }

    public DateTime PlannedDeliveryDate { get; set; }

    public DateTime DeliveryDate { get; set; }

    public string Phase { get; set; } = default!;

    public string Ext { get; set; } = default!;

    public string Street { get; set; } = default!;

    public string Address { get; set; }=default!;

    public decimal latitude { get; set; }

    public decimal longitude { get; set; }

    public Guid CapacityID { get; set; }

    public OLH_BowserCapacity BowserCapacity { get; set; }=default!;

    public decimal Amount { get; set; }

    public long PaymentID { get; set; }

    public Guid DriverId { get; set; }

    public OLH_DriverInfo DriverInfo { get; set; } = default!;


    public Guid VehicleId { get; set; }

    public OLH_Vehicle Vehicle { get; set; } =default!;

    public int PaymentStatusID { get; set; }

    public Guid RequestStatusId { get; set; }  
    






}

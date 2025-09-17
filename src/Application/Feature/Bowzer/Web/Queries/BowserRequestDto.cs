using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries;
public class BowserRequestDto
{
    public Guid Id { get; set; }
    public string RequestNo { get; set; } = default!;
    public DateTime RequestDate { get; set; }

    public Guid PhaseId { get; set; }
    public string? PhaseName { get; set; }

    public Guid BowserCapacityId { get; set; }
    public string? BowserCapacityName { get; set; }

    public DateTime RequestedDeliveryDate { get; set; }
    public DateTime? PlannedDeliveryDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    public string? Ext { get; set; }
    public string Street { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Guid? AssignedDriverId { get; set; }
    public string? AssignedDriverName { get; set; }

    public Guid? AssignedVehicleId { get; set; }
    public string? AssignedVehiclePlate { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;

    public BowserStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public string? CustomerId { get; set; }
    public string? Notes { get; set; }
}


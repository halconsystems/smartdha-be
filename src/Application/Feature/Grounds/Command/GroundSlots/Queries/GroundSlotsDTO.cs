using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Queries;

public class GroundSlotsdto
{
    public Guid GroundId {  get; set; }
    public Guid Id { get; set; }
    public string? SlotName { get; set; }
    public string? DisplayName { get; set; }
    public string? Code { get; set; }
    public string? SlotPrice { get; set; }
    public DateTime FromDate { get; set; }
    public DateOnly FromDateOnly { get; set; }
    public TimeOnly FromTimeOnly { get; set; }
    public DateTime ToDate { get; set; }
    public DateOnly ToDateOnly { get; set; }
    public TimeOnly ToTimeOnly { get; set; }
    public AvailabilityAction Action { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomWithServiceSelections;
public class RoomWithServicesDto
{
    public RoomBriefDto Room { get; set; } = default!;
    public List<ServiceSelectionDto> Services { get; set; } = new();
}

public class RoomBriefDto
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string ClubName { get; set; } = default!;
    public Guid RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; } = default!;
    public Guid ResidenceTypeId { get; set; }
    public string ResidenceTypeName { get; set; } = default!;
    public string No { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsGloballyAvailable { get; set; }
}
public class ServiceSelectionDto
{
    public Guid ServiceId { get; set; }
    public string Name { get; set; } = default!;
    public bool Selected { get; set; }   // true if mapped to the room
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;

    public Guid RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; } = string.Empty;

    public Guid ResidenceTypeId { get; set; }
    public string ResidenceTypeName { get; set; } = string.Empty;

    public string No { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsGloballyAvailable { get; set; }
}


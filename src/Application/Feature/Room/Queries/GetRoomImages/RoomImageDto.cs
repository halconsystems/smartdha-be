using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomImages;
public class RoomImageDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string ImageURL { get; set; } = default!;
    public string ImageExtension { get; set; } = default!;
    public string? ImageName { get; set; }
    public string? Description { get; set; }
    public ImageCategory Category { get; set; }
}


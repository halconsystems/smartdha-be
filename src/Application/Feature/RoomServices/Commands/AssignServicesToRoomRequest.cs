using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomServices.Commands;
public sealed class AssignServicesToRoomRequest
{
    public Guid RoomId { get; set; }
    public List<Guid> ServiceIds { get; set; } = new();
}

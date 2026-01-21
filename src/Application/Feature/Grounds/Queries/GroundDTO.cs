using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Queries;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;

public class GroundDTO
{
    public Guid Id { get; set; }
    public Guid? ClubId { get; set; }
    public string? ClubName { get; set; } 

    public GroundCategory GroundCategory { get; set; }
    public GroundType GroundType { get; set; }
    public string GroundName { get; set; } = string.Empty;

    public string? GroundDescription { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? AccountNo { get; set; }
    public string? AccountNoAccronym { get; set; }
    public string? MainImageUrl { get; set; }
    public string? SlotCount { get; set; }
    public List<GroundImagesDTO>? GroundImages { get; set; }
    public List<GroundSlotsdto>? Slots { get; set; }
    public List<GroundStandtardTime>? GroundStandtardTimes { get; set; }
    public GroundStandtardTime? GroundStandtardTime { get; set; }
}

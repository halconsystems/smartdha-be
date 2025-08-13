using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Queries.GetRoomAvailabilities;
public record RoomAvailabilityDto(
    Guid Id,
    Guid RoomId,
    string RoomNo,
    string? RoomName,
    Guid ClubId,
    string ClubName,
    Guid RoomCategoryId,
    string RoomCategoryName,
    Guid ResidenceTypeId,
    string ResidenceTypeName,
    DateTime FromDate,
    DateTime ToDate,
    AvailabilityAction Action,
    string? Reason,
    bool IsGloballyAvailable
);



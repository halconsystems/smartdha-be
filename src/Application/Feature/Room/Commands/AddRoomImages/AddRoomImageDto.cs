using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
public record AddRoomImageDto(
    string ImageURL,
    string ImageExtension,
    string? ImageName,
    string? Description,
    ImageCategory Category
);


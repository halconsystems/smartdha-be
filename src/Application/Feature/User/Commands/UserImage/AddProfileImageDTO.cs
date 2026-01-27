using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UserImage;

public record AddProfileImageDTO(
    string ImageURL,
    string ImageExtension,
    string? ImageName,
    string? Description,
    ImageCategory Category
);

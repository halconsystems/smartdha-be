using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UserImage;

public class AddUserImagesFlatForm
{
    // One file per image (Swagger shows proper file pickers for this)
    [Required]
    public IFormFile Files { get; set; } = default!;

    // Parallel arrays (optional, same count/order as Files)
    public string? ImageNames { get; set; }
    public string? Descriptions { get; set; }

    // Send enum values (numbers) to keep it simple in Swagger/Postman
    public ImageCategory Categories { get; set; }
}

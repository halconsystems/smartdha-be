using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Command;

public class AddGroundImageDTO
{
    // One file per image (Swagger shows proper file pickers for this)
    [Required]
    public List<IFormFile> Files { get; set; } = new();

    // Parallel arrays (optional, same count/order as Files)
    public List<string?> ImageNames { get; set; } = new();
    public List<string?> Descriptions { get; set; } = new();

    // Send enum values (numbers) to keep it simple in Swagger/Postman
    public List<ImageCategory> Categories { get; set; } = new();
}


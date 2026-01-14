using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public class MemberSpouseDTO
{
    public Guid MemberShipId { get; set; }
    public MemberRequest? MemberRequest { get; set; }

    [Required]
    public string FullName { get; set; } = default!;
    public string? MobileNo { get; set; }
    public string? Email { get; set; }
    [Required]
    public IFormFile PicturePath { get; set; } = default!;
    [Required]
    public IFormFile CnicFrontImage { get; set; } = default!;

    [Required]
    public IFormFile CnicBackImage { get; set; } = default!;
    
    public Nationality Nationality { get; set; }
    public string? otherNationality { get; set; }

    [Required]
    public string Cnic { get; set; } = default!;

    [Required]
    public DateTime CnicExpiry { get; set; } = default!;
}

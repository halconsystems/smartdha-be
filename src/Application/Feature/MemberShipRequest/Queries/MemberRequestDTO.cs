using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public class MemberRequestDTO
{
    public Guid MemberShipCategory { get; set; }
    public MemberShipCatergories? MemberShipCatergories { get; set; }
    
    public string? Purpose { get; set; }
    public string? CNIC { get; set; } = default!;
   
    public string? Name { get; set; }
   
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDelete { get; set; }

}

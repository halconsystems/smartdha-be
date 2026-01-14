using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class MemberChildren : BaseAuditableEntity
{
    public Guid MemberShipId { get; set; }
    public MemberRequest? MemberRequest { get; set; }

    [Required]
    public string FullName { get; set; } = default!;
    public Relation Relation { get; set; }
    public string? MobileNo { get; set; }
    public bool IsAdult { get; set; }
    [MaxLength(500)]
    public string? PicturePath { get; set; }
    [MaxLength(500)]
    public string? CNICFrontImagePath { get; set; }

    [MaxLength(500)]
    public string? CNICBackImagePath { get; set; }
    public string? CnicNo { get; set; }
    public DateTime CnicExpiryDate { get; set; }


    [MaxLength(500)]
    public string? NadraBForm { get; set; }
    public string? HusbandName { get; set; }

}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Domain.Entities.GBMS;

public class Grounds : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? AccountNo { get; set; }
    [MaxLength(4)] public string? AccountNoAccronym { get; set; }
    public GroundType GroundType { get; set; }
    public GroundCategory GroundCategory { get; set; }
    public Guid? ClubId { get; set; }
    public Club? Club { get; set; }
}

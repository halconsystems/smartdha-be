using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class DHAClub : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? AccountNo { get; set; }
    [MaxLength(4)] public string? AccountNoAccronym { get; set; }
    public string? MarchantCode { get; set; }
    public string? Email { get; set; }
}

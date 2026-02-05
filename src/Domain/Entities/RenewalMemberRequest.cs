using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class RenewalMemberRequest :BaseAuditableEntity
{
    [Required]
    public string MemPk { get; set; } = default!; 

    public VerificationStatus VerificationStatus { get; set; }
    public DateOnly ValidUntil {  get; set; }
    public Guid ApprovedBy { get; set; }

    public string? PresentAddress {  get; set; }
    public string? permenantAddress {  get; set; }

}

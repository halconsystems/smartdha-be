using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NonMemberVerificationDocument : BaseAuditableEntity
{

    [Required]
    public Guid VerificationId { get; set; }
    public NonMemberVerification Verification { get; set; } = default!;

    [MaxLength(500)]
    public string? CNICFrontImagePath { get; set; }

    [MaxLength(500)]
    public string? CNICBackImagePath { get; set; }

    [MaxLength(500)]
    public string? ProfilePicture { get; set; } 

    [MaxLength(500)]
    public string? UtilityBill { get; set; }

    [MaxLength(500)]
    public string? SupportingDocumentPath { get; set; } // optional 

    [MaxLength(100)]
    public string? DocumentType { get; set; } // optional, e.g., "Utility Bill", "Rental Proof"

    [MaxLength(250)]
    public string? OriginalFileName { get; set; } // optional: store uploaded file name
}


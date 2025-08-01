using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DHAFacilitationAPIs.Domain.Entities;
public class NonMemberVerification : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;

    [Required]
    public string CNIC { get; set; } = default!;

    public string Remarks { get; set; } = default!;
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public DateTime? ApprovedAt { get; set; } = default!;
    public string? ApprovedBy { get; set; } = default!;// Admin UserId
}


using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    
    [Required]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "CNIC must be 13 digits.")]
    public string CNIC { get; set; } = default!;

    [Required]
    [RegularExpression(@"^\d{11,15}$", ErrorMessage = "Mobile number must be between 11 and 15 digits.")]
    public string MobileNo { get; set; } = default!;

    public string MEMPK { get; set; } = default!;

    [Required]
    public AppType AppType { get; set; }  // <-- Web or Mobile
    [Required]
    public UserType UserType { get; set; }  // <-- Membeership or Non Member and Employee

    //From DHA Database
    public string? RegisteredMobileNo { get; set; } = default!;
    public string? RegisteredEmail { get; set; } = default!;
    public string? RegistrationNo { get; set; } = default!;
    public DateTime? RegistrationDate { get; set; } = default!;

    public ICollection<UserModuleAssignment> ModuleAssignments { get; set; } = default!;
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public bool IsOtpRequired { get; set; } = false;

    

}

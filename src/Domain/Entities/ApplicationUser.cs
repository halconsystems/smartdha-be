
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
    public UserType UserType { get; set; }  // <-- Web or Mobile
    public ICollection<UserModuleAssignment> ModuleAssignments { get; set; } = default!;
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

}

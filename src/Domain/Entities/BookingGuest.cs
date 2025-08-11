using System.ComponentModel.DataAnnotations;

public class BookingGuest : BaseAuditableEntity
{
    [Required, MaxLength(200)] public string FullName { get; set; } = default!;
    [MaxLength(50)] public string? CNICOrPassport { get; set; }
    [MaxLength(50)] public string? Phone { get; set; }
    [MaxLength(200)] public string? Email { get; set; }
    public string? Address { get; set; }
}

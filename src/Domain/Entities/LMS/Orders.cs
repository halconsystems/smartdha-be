using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class Orders :BaseAuditableEntity
{
    [Required]
    public string UniqueFormID { get; set; } = default!;
    public Guid ServiceId { get; set; }
    public LaundryService? LaundryService { get; set; }
    public Guid PackageId { get; set; }
    public LaundryPackaging? LaundryPackaging { get; set; }
    [Required]
    public string UserId { get; set; } = default!;
    public OrderType OrderType { get; set; }

}

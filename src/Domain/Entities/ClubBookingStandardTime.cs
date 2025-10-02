using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ClubBookingStandardTime : BaseAuditableEntity
{
    [Required]
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;
    [Required] public TimeOnly CheckInTime { get; set; }      
    [Required] public TimeOnly CheckOutTime { get; set; }      
}

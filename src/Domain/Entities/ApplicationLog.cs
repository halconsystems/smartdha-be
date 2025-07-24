using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ApplicationLog : BaseEntity
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? DeviceId { get; set; } = default!;
    public UserType UserType { get; set; }
    public string Action { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool IsSuccessfullLogin { get; set; } = false;
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
}

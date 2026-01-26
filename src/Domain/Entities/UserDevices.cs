using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class UserDevices : BaseAuditableEntity
{
    public Guid UserId {  get; set; }
    public string? DeviceId { get; set; }
    public string? FCMToken {  get; set; }
    public DateTime LoginAt { get; set; }
}

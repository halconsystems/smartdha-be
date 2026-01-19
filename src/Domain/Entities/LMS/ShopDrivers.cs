using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class ShopDrivers :BaseAuditableEntity
{
    public Guid ShopId { get; set; }
    public Shops? Shops { get; set; }
    public Guid DriverId    { get; set; }


}

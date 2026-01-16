using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class PaymentDTSetting :BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Orders? Orders { get; set; }
    public Guid OrderDTiD { get; set; }
    public OrderDTSetting? OrderDTSetting { get; set; }
    public bool IsDiscount {  get; set; }
}

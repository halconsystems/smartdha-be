using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;

public class ShopDTdto
{
    public Guid Id { get; set; }    
    public Guid ShopId { get; set; }
    public Domain.Entities.LMS.Shops? Shop { get; set; }
    public Domain.Entities.LMS.OrderDTSetting? OrderDTSetting { get; set; }
    public Guid OrderDTId { get; set; }
    public string? Value { get; set; }

}

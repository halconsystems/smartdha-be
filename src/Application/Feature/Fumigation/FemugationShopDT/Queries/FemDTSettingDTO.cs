using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Queries;

public class FemDTSettingDTO
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public Domain.Entities.FMS.FemgutionShops? Shop { get; set; }
    public Domain.Entities.FMS.FemDTSetting? FemDTSettings { get; set; }
    public Guid FemDTId { get; set; }
    public string? Value { get; set; }
}

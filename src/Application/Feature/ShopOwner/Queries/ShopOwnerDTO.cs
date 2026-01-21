using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ShopOwner.Queries;

public class ShopOwnerDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public bool IsActive { get; set; }
}



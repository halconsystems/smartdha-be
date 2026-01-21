using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;


public class MemberShipDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Code { get; set; }
}

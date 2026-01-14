using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class MemberShipCatergories : BaseAuditableEntity
{
    public string? name { get; set; }
    public string? displayname { get; set; }
    public string? Code { get; set; }
    public Guid MemberShipId { get; set; }
    public MemberShips? MemberShips { get; set; }
}


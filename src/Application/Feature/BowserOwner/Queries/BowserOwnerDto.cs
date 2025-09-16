using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserOwner.Queries;
public class BowserOwnerDto
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; } = default!;
    public bool? IsActive { get; set; }
}


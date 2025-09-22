using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;
public class BowserMakeDto
{
    public Guid Id { get; set; }
    public string MakeName { get; set; } = default!;
}


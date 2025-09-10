using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserModel.Queries;

public class BowserModelDto
{
    public Guid Id { get; set; }
    public string ModelName { get; set; } = default!;
    public Guid MakeId { get; set; }
    public string MakeName { get; set; } = default!;
}

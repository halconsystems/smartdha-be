using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class NumberSequence
{
    public int Id { get; set; }

    public string Prefix { get; set; } = default!;

    public DateOnly SequenceDate { get; set; }

    public int LastNumber { get; set; }
}


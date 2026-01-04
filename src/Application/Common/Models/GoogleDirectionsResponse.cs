using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;
public class GoogleDirectionsResponse
{
    public List<Route>? routes { get; set; }
}

public class Route
{
    public List<Leg>? legs { get; set; }
}

public class Leg
{
    public GoogleValue distance { get; set; } = default!;
    public GoogleValue duration { get; set; } = default!;
    public GoogleValue? duration_in_traffic { get; set; }
}

public class GoogleValue
{
    public string text { get; set; } = default!;
    public int value { get; set; } // in seconds or meters
}

public class GoogleRouteResult
{
    public double DistanceKm { get; set; }
    public double DurationMinutes { get; set; }
}


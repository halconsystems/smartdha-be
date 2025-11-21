using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Infrastructure.Service.Geocoding;
public class GoogleMapsOptions
{
    public const string SectionName = "GoogleMaps";
    public string ApiKey { get; set; } = default!;
}

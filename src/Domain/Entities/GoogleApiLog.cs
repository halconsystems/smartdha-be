using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class GoogleApiLog : BaseAuditableEntity
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ResponseStatus { get; set; }
    public string? FormattedAddress { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsSuccess { get; set; }
}


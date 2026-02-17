using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.CreateLaggagePass;

public class CreateLaggagePassCommand : IRequest<CreateLaggagePassResponse>
{
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ValidityDate { get; set; }
}

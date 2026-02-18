using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.CreateLuggagePass;

public class CreateLuggagePassCommand : IRequest<Result<CreateLuggagePassResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ValidityDate { get; set; }
}

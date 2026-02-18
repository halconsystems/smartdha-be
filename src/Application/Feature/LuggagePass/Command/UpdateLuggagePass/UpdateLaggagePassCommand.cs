using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.UpdateLuggagePass;

public class UpdateLuggagePassCommand : IRequest<Result<UpdateLuggagePassResponse>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}

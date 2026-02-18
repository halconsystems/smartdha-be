using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.CreateVisitorPass;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.UpdateVisitorPass;

public class UpdateVisitorPassCommand : IRequest<Result<UpdateVisitorPassResponse>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; }
    public VisitorPassType VisitorPassType { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle.UpdateVecleCommandHandler;

public class UpdateVehicleCommand : IRequest<UpdateVehicleResponse>
{
    public Guid Id { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public IFormFile? Attachment { get; set; }  
}

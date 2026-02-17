using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;

public class CreateVehicleCommand : IRequest<CreateVehicleResponse>
{
    public int LicenseNo { get; set; }
    public string License { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public IFormFile? Attachment { get; set; }  
    public string? ETagId { get; set; }
    public DateTime? ValidUpTo { get; set; }
}

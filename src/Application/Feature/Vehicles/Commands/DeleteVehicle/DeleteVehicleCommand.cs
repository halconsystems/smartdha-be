using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle;
using MediatR;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Command;

public class DeleteVehicleCommand : IRequest<DeleteVehicleResponse>
{
    public Guid Id { get; set; }
}

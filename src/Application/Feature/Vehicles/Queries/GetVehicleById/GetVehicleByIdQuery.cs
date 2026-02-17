using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleById;
using MediatR;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries;

public class GetVehicleByIdQuery : IRequest<GetVehicleByIdResponse>
{
    public Guid Id { get; set; }

}

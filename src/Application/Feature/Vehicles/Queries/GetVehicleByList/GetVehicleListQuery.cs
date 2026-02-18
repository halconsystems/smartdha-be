using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleByList;

public class GetVehicleListQuery : IRequest<List<Domain.Entities.Smartdha.Vehicle>>
{
    public string Id { get; set; } = string.Empty;
}

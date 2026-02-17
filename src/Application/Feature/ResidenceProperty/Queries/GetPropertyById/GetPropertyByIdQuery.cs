using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyById;

namespace DHAFacilitationAPIs.Application.Feature.Property.Queries;

public class GetPropertyByIdQuery : IRequest<GetPropertyByIdResponse>
{
    public Guid Id { get; set; }
}


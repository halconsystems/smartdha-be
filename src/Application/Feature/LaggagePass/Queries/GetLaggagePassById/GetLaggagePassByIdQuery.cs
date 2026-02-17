using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Queries.GetLaggagePassById;

public class GetLaggagePassByIdQuery : IRequest<GetLaggagePassByIdResponse>
{
    public Guid Id { get; set; }
}

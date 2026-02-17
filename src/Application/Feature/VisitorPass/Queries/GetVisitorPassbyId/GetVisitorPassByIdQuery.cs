using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;

public class GetVisitorPassByIdQuery : IRequest<GetVisitorPassByIdResponse>
{
    public Guid Id { get; set; }
}

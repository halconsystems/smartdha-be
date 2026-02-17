using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetLuggagePassById;

public class GetLuggagePassByIdQuery : IRequest<GetLuggagePassByIdResponse>
{
    public Guid Id { get; set; }
}

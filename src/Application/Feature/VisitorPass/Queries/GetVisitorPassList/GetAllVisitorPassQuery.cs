using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassList;

public class GetAllVisitorPassQuery : IRequest<List<GetVisitorPassByIdResponse>>
{
    public string Id { get; set; }= string.Empty;
}

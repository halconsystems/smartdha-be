using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetWorkerById;
public class GetWorkerByIdQuery : IRequest<Result<GetWorkerByIdResponse>>
{
    public Guid Id { get; set; }
}

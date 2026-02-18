using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetAllWorkers;
public class GetAllWorkerQuery : IRequest<Result<List<GetAllWorkerQueryResponse>>>
{
    public string Id { get; set; } = null!;
}

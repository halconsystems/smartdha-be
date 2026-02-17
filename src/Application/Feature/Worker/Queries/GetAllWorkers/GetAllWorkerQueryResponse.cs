using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetAllWorkers;
public class GetAllWorkerQueryResponse
{
    public string? Name { get; set; }
    public string? CNIC { get; set; }
    public string? PhoneNo { get; set; }
    public DateTime DOB { get; set; }
    public string? WorkerCardNo { get; set; }
    public bool PoliceVerification { get; set; }
    public string? Image { get; set; }
    public JobType? JobType { get; set; }
}

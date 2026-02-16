using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Commands.UpdateWorker;
public class UpdateWorkerResponse 
{
    public Guid WorkerId { get; set; }
    public JobType JobType { get; set; }
    public  string? FatherHusbandName { get; set; }
    public  string? Name { get; set; }
    public  string? PhoneNo { get; set; }
    public  string? CNIC { get; set; }
    public DateTime DOB { get; set; }
    public string? CnicFront { get; set; }
    public string? CnicBack { get; set; }
    public string? ProfilePicture { get; set; }
    public bool? PoliceVerification { get; set; }
}

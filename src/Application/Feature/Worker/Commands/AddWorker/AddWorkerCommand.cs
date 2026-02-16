using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Commands.AddWorker;
public class AddWorkerCommand : IRequest<Result<Guid>>
{
    public required string Name { get; set; }
    public JobType JobType { get; set; }
    public required string FatherHusbandName { get; set; }
    public required string PhoneNo { get; set; }
    public required string CNIC { get; set; }
    public DateTime DOB { get; set; }
    public IFormFile? CnicFront { get; set; }
    public IFormFile? CnicBack { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public required bool PoliceVerification { get; set; }
    public IFormFile? PoliceVerificationAttachment { get; set; }
    public int WorkerCardDeliveryType { get; set; }
    public Guid? Address { get; set; }
}

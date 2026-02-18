using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Commands.UpdateWorker;
public class UpdateWorkerCommand : IRequest<Result<UpdateWorkerResponse>>
{
    public Guid WorkerId { get; set; }
    public int JobType { get; set; }
    public string? FatherHusbandName { get; set; }
    public string? Name { get; set; }
    public string? PhoneNo { get; set; }
    public string? CNIC { get; set; }
    public DateTime DOB { get; set; }
    public IFormFile? CnicFront { get; set; }
    public IFormFile? CnicBack { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public required bool PoliceVerification { get; set; }
    public IFormFile? PoliceVerificationAttachment { get; set; }
    public int WorkerCardDeliveryType { get; set; }
}

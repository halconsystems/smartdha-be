using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Domain.Entities.Smartdha;
public class Worker : BaseAuditableEntity
{
    public required JobType JobType { get; set; }
    public required string Name { get; set; } 
    public required string FatherOrHusbandName { get; set; } 
    public required string PhoneNumber { get; set; }
    public required string CNIC { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public string? CnicFront { get; set; } 
    public string? CnicBack { get; set; } 
    public string? ProfilePicture { get; set; } 
    public bool? PoliceVerification { get; set; } 
    public string? PoliceVerificationAttachment { get; set; } 
    public string? WorkerCardNumber { get; set; } 
    public WorkerCardDeliveryType WorkerCardDeliveryType { get; set; } 
    public DateTime? ValidTo { get; set; } 
    public DateTime? ValidFrom { get; set; } 
}

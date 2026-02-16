using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.UpdateUserFamilyCommandHandler;
public class UpdateUserFamilyResponse
{
    public string Message { get; set; } = "";
    public string Name { get; set; } = "";
    public string PhoneNo { get; set; } = "";
    public string CNIC { get; set; } = "";
    public RelationUserFamily Relation { get; set; } 
    public DateTime DOB { get; set; }
    public string? ProfilePicture { get; set; }
}

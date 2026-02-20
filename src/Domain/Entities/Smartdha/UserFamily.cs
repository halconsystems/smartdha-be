using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.Smartdha;
public class UserFamily : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ResidentCardNumber { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Cnic { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FatherOrHusbandName { get; set; }
    public RelationUserFamily Relation { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime? ValidTo { get; set; }
    public DateTime? ValidFrom { get; set; }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;

public class GetUserFamilybyIdQueryResponse
{
    public string Name { get; set; } = string.Empty;
    public RelationUserFamily? Relation { get; set; } 
    public string Phone { get; set; } = string.Empty;
    public DateTime DOB { get; set; }
    public string ResidentCardNumber { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserWithAccess;
public class UserAccessWithInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public List<ModuleTreeDto> Modules { get; set; } = new();
    public List<ClubsDto> Clubs { get; set; } = new();
}
public class ClubsDto
{
    public Guid ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;
}




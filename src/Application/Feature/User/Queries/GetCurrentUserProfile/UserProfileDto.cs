using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetCurrentUserProfile;
public class UserProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string CNIC {  get; set; } = string.Empty ;
    public string MobileNo {  get; set; } = string.Empty ;
}


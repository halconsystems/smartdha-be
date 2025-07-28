using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class AuthenticationVM
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class AuthenticationDto
{
    public string Role { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string ResponseMessage { get; set; } = default!;
}

public class MobileAuthenticationDto
{
    public string Name { get; set; } = default!;
    public Boolean isOtpRequired { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string ResponseMessage { get; set; } = default!;
    public string MobileNumber { get; set; } = default!;
    
}

public class OtpAuthenticationDto
{
    public string Name { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string ResponseMessage { get; set; } = default!;
    public string MobileNumber { get; set; } = default!;
    public bool isOtpVerified { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string UserType { get; set; } = default!;
}

public class MobileModule
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
}

public class SecretKeyDto
{
    public string SecretKey { get; set; } = default!;
}

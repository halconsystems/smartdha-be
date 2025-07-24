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
    public string AccessToken { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string ResponseMessage { get; set; } = default!;
}

public class SecretKeyDto
{
    public string SecretKey { get; set; } = default!;
}

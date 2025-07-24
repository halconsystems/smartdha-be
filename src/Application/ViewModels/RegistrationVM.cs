using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class RegistrationVM
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public class ForgotPasswordVM
{
    public string Email { get; set; } = default!;
}
public class ResetPasswordVM
{
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}

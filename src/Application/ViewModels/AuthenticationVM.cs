using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class AuthenticationVM
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class AuthenticationDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
    public string Name {  get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<ModuleTreeDto> Modules { get; set; } = new(); // include module/submodule with permissions
}

public class ModuleDto
{
    public Guid ModuleId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string? ModuleURL {  get; set; } = string.Empty;
    public List<SubModuleDto> SubModules { get; set; } = new();
}

public class SubModuleDto
{
    public Guid SubModuleId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string SubModuleName { get; set; } = string.Empty;
    public List<AllPermissionDto> Permissions { get; set; } = new();
}
public class AllPermissionDto
{
    public Guid PermissionId { get; set; }
    public string Value { get; set; } = string.Empty;       // e.g. "Approve"
    public string DisplayName { get; set; } = string.Empty; // e.g. "Approve Request"
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

public class RegisterationDto
{
    public Boolean isOtpRequired { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string ResponseMessage { get; set; } = default!;
    public string MobileNumber { get; set; } = default!;
}

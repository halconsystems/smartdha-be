using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DHAFacilitationAPIs.Web.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _action;       // "Read", "Write", "Delete"
    private readonly string _subModuleId;

    public AuthorizePermissionAttribute(string action, string subModuleId)
    {
        _action = action;
        _subModuleId = subModuleId;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var expectedClaim = $"{_action}:{_subModuleId}";

        var hasPermission = user.Claims
            .Any(c => c.Type == "Permission" && c.Value.Equals(expectedClaim, StringComparison.OrdinalIgnoreCase));

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}


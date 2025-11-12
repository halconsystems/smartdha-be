using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MobileAPI.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class ModuleAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _moduleIds;

    public ModuleAuthorizeAttribute(params string[] moduleIds)
    {
        _moduleIds = moduleIds;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 🔓 Allow Admin/SuperAdmin
        if (user.IsInRole("Admin") || user.IsInRole("SuperAdmin"))
            return;

        var moduleClaims = user.FindAll("ModuleAccess").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!_moduleIds.Any(id => moduleClaims.Contains(id)))
        {
            context.Result = new ForbidResult();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


namespace DHAFacilitationAPIs.Web.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeModuleAccessAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _moduleId;

    public AuthorizeModuleAccessAttribute(string moduleId)
    {
        _moduleId = moduleId;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var hasAccess = user.HasClaim("ModuleAccess", _moduleId);

        if (!hasAccess)
        {
            context.Result = new ForbidResult();
        }
    }
}


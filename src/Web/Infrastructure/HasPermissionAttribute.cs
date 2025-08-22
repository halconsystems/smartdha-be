using System.Security.Claims;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Permissions.Queries.GetUserPermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DHAFacilitationAPIs.Web.Infrastructure;

public class HasPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string _permission;

    public HasPermissionAttribute(string permission) => _permission = permission;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var cache = context.HttpContext.RequestServices.GetRequiredService<IPermissionCache>();
        var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();

        // Use Redis → fallback to DB
        var permissions = await cache.GetPermissionsAsync(userId, async () =>
        {
            return await mediator.Send(new GetUserPermissionsQuery(userId));
        });

        if (!permissions.Contains(_permission))
        {
            context.Result = new ForbidResult();
        }
    }
}


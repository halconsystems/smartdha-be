using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[ApiController]
public class BaseApiController : ControllerBase
{
    private ISender _mediator = null!;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    protected string Identity
    {
        get
        {
            return User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault()!;
        }
    }

}

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class EnumController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    private readonly IEnumService _enumService;

    public EnumController(IMediator mediator, IUser loggedInUser, IEnumService enumService)
    {
        _mediator = mediator;
        _loggedInUser = loggedInUser;
        _enumService = enumService;
    }

    [HttpGet, AllowAnonymous]
    public IActionResult GetEnums([FromQuery] EnumType? type)
    {
        var result = _enumService.GetEnums(type);
        return Ok(result);
    }
}


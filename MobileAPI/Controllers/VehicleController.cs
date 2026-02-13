using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "smartdha")]
[ApiController]
public class VehicleController : BaseApiController
{
    private readonly IUser _loggedInUser;
    private readonly IMediator _mediator;
    public VehicleController(IUser loggedInUser, IMediator mediator)
    {
        _loggedInUser = loggedInUser;
        _mediator = mediator;
    }

}

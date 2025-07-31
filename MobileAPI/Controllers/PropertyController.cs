using DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Commands;
using DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyController : BaseApiController
{
    [HttpGet("Get-properties")]
    public async Task<IActionResult> GetMyProperties()
    {
        return Ok(await Mediator.Send(new GetMyPropertiesQuery()));
        
    }
   

}

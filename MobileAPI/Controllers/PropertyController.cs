using DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Commands;
using DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
using DHAFacilitationAPIs.Application.Feature.PropertyDetails.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace MobileAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "property")]
public class PropertyController : BaseApiController
{
    [HttpGet("Get-properties")]
    public async Task<IActionResult> GetMyProperties()
    {
        return Ok(await Mediator.Send(new GetMyPropertiesQuery()));
        
    }
    [HttpGet("Get-propertiesDetails")]
    public async Task<IActionResult> GetMyProperties(string Cnic)
    {
        return Ok(await Mediator.Send(new GetAllProperyByCnicQuery(Cnic)));
        
    }
   

}

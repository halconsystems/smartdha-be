using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.Property.Queries;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.CreateProperty;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.DeleteProperty;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.UpdateProperty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "smartdha")]
    [ApiController]
    public class ResidencePropertyController : BaseApiController
    {
        private readonly IUser _loggedInUser;
        private readonly IMediator _mediator;

        public ResidencePropertyController(IUser loggedInUser, IMediator mediator)
        {
            _loggedInUser = loggedInUser;
            _mediator = mediator;
        }

        [HttpPost("create"), AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProperty([FromForm] CreatePropertyCommand request)
        {         
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("update"), AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProperty([FromForm] UpdatePropertyCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("delete"), AllowAnonymous]
        public async Task<IActionResult> DeleteProperty([FromBody] DeletePropertyCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("get-property-by-id/{id}"), AllowAnonymous]
        public async Task<IActionResult> GetProperty(Guid id)
        {
            var result = await _mediator.Send(new GetPropertyByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpPost("get-all-properties"), AllowAnonymous]
        public async Task<IActionResult> GetAllProperties(GetAllPropertiesQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}

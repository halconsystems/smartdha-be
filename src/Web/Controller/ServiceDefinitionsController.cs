using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands.CreateServiceDefinition;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands.DeleteServiceDefinition;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands.UpdateServiceDefinition;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Queries.GetAllServiceDefinitions;
using DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Queries.GetServiceDefinitionById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/cbms/service-definitions")]
[ApiExplorerSettings(GroupName = "CBMS")]

public class ServiceDefinitionsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ServiceDefinitionsController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceDefinitionDto dto)
        => Ok(await _mediator.Send(new CreateServiceDefinitionCommand(dto)));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateServiceDefinitionDto dto)
        => Ok(await _mediator.Send(new UpdateServiceDefinitionCommand(dto)));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => Ok(await _mediator.Send(new DeleteServiceDefinitionCommand(id)));

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new GetAllServiceDefinitionsQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _mediator.Send(new GetServiceDefinitionByIdQuery(id)));
}

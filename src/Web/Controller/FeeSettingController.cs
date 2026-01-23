using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Commands.CreateFeeSetting;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Commands.DeleteFeeSetting;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Commands.UpdateFeeSetting;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Queries.GetFeeSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "property")]
public class FeeSettingController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeeSettingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFeeSettingCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateFeeSettingCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteFeeSettingCommand(id), ct));

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _mediator.Send(new GetFeeSettingsQuery(), ct));
}


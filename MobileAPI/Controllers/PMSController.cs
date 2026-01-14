using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries;
using DHAFacilitationAPIs.Application.Feature.PMS.Commands.AddCase;
using DHAFacilitationAPIs.Application.Feature.PMS.Commands.AddCaseAttachment;
using DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetPriority;
using DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcesses;
using DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcessPrerequisites;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PMSController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _files; 
    private readonly ICurrentUserService _current;

    public PMSController(IMediator mediator, IFileStorageService files, ICurrentUserService current)
    {
        _mediator = mediator;
        _files = files;
        _current = current;
    }
    
    [HttpPost("AddCase")]
    public async Task<IActionResult> AddCase(AddCaseCommand command)
       => Ok(await _mediator.Send(command));

    [AllowAnonymous]
    [HttpGet("GetProcess")]
    public async Task<IActionResult> GetProcess()
        => Ok(await _mediator.Send(new GetProcessTypesQuery(AppType.Mobile)));
    
    [HttpGet("GetProcessPrerequisites")]
    public async Task<IActionResult> GetProcessPrerequisites(
    
    [FromQuery] int processId)
    {
        var userId = _current.UserId;

        if (userId == Guid.Empty)
            return Unauthorized();
        var result = await _mediator.Send(
            new GetProcessPrerequisitesQuery(userId, processId));

        return Ok(result);
    }

    [HttpGet("priorities")]
    public async Task<IActionResult> GetPriorities(
    [FromQuery] int appId,
    [FromQuery] int processId,
    CancellationToken ct)
    {
        var userId =  _current.UserId;

        if (userId == Guid.Empty)
            return Unauthorized();

        var result = await _mediator.Send(
            new GetPrioritiesQuery(userId, appId, processId),
            ct);

        return Ok(result);
    }


    
    [HttpPost("addattachment")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> AddCaseAttachments(
         [FromForm] AddCaseAttachmentsForm form,
         CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (form.Files.Count != form.PrerequisitesIDs.Count)
            return BadRequest("Files and PrerequisitesIDs count mismatch.");

        var userId = _current.UserId;

        SuccessResponse<string>? lastResult = null;

        var folder = $"Cases/{form.CaseId}";

        for (int i = 0; i < form.Files.Count; i++)
        {
            // ✅ Save file in controller
            var filePath = await _files.SaveFileAsync(
                form.Files[i],
                folder,
                ct,
                maxBytes: 10 * 1024 * 1024,
                allowedExtensions: new[] { ".jpg", ".jpeg", ".png", ".pdf" }
            );

            // ✅ Send only file path to CQRS
            var cmd = new AddCaseAttachmentCommand(
                userId,
                form.CaseId,
                form.PrerequisitesIDs[i],
                filePath
            );

            lastResult = await _mediator.Send(cmd, ct);
        }

        return Ok(lastResult ?? new SuccessResponse<string>("No attachments processed."));
    }


}

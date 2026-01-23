using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;


[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "laundry")]
public class LaundryItemsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _files;
    public LaundryItemsController(IMediator mediator, IFileStorageService files)
    {
        _mediator = mediator;
        _files = files;
    }
    [HttpPost("Create-LaundryItems"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> CreateLaundryItems(CreateLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("Update-LaundryItems")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryItems(Guid id, ModifyLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpGet("get-LaundryItems"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllLaundryItems(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLaundryItemsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("get-LaundryItems-ById"), AllowAnonymous]
    public async Task<ActionResult<MemberShipDTO>> GetAllReligonSectById(Guid CategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLaundryItemByIdQuery(CategoryId), ct);
        return Ok(result);
    }

    [HttpPost("{roomId:guid}/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddLaundryImage(
   Guid LaundryId,
   [FromForm] AddLaundryItemForm form,
   CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (form.Files == null)
            return BadRequest("No images uploaded.");

        // Keep parallel arrays aligned (if provided)
        if (form.ImageNames == null)
            return BadRequest("ImageNames count must match Files count.");
        if (form.Descriptions == null)
            return BadRequest("Descriptions count must match Files count.");

        var folder = $"LaundryItems/{LaundryId}";

        var file = form.Files;

        // Save file (your IFileStorageService validates size/ext & creates folder)
        //var relativePath = await _files.SaveFileAsync(file, folder, ct);
        var relativePath = await _files.SaveFileAsync(
          file,
          folder,
          ct,
          maxBytes: 5 * 1024 * 1024,                       // 5 MB
          allowedExtensions: new[] { ".jpg", ".jpeg", ".png" }
         );

        var ext = Path.GetExtension(relativePath);

        // Pick metadata by index (or defaults)
        var name = form.ImageNames;
        var desc = form.Descriptions;
        var cat = form.Categories;

        var uploaded = new AddLaundryImageDTO(
        ImageURL: relativePath,
        ImageExtension: ext,
        ImageName: name,
        Description: desc,
        Category: ImageCategory.Main
    );

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddLaundryImagesCommand(LaundryId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpDelete("Delete-LaundryItems"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteLaundryItemsCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("Active-Inactive-LaundryItems")]
    public async Task<ActionResult<SuccessResponse<string>>> UpdateLaundryCategory(Guid id,bool Active, ActiveInActiveLaundryItemsCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd with { Id = id }, ct));
}

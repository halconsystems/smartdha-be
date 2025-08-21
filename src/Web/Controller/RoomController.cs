using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomCharges;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.CreateRoom;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.DeleteRoom;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.UpdateRoom;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetImageCategories;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomImages;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomWithServiceSelections;
using DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.CreateRoomCategory;
using DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomServices.Commands;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _files;
    public RoomController(IMediator mediator, IFileStorageService files)
    {
        _mediator = mediator;
        _files = files;
    }

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateRoomCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("image-categories"), AllowAnonymous]
    public async Task<IActionResult> GetImageCategories(CancellationToken ct)
         => Ok(await _mediator.Send(new GetImageCategoriesQuery(), ct));


    [HttpPost("{roomId:guid}/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddRoomImages(
    Guid roomId,
    [FromForm] AddRoomImagesFlatForm form,
    CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (form.Files == null || form.Files.Count == 0)
            return BadRequest("No images uploaded.");

        // Keep parallel arrays aligned (if provided)
        int n = form.Files.Count;
        if (form.ImageNames.Count != 0 && form.ImageNames.Count != n)
            return BadRequest("ImageNames count must match Files count.");
        if (form.Descriptions.Count != 0 && form.Descriptions.Count != n)
            return BadRequest("Descriptions count must match Files count.");
        if (form.Categories.Count != 0 && form.Categories.Count != n)
            return BadRequest("Categories count must match Files count.");

        var folder = $"Rooms/{roomId}";
        var uploaded = new List<AddRoomImageDto>();

        for (int i = 0; i < n; i++)
        {
            var file = form.Files[i];

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
            var name = form.ImageNames.ElementAtOrDefault(i);
            var desc = form.Descriptions.ElementAtOrDefault(i);
            var cat = form.Categories.ElementAtOrDefault(i);

            uploaded.Add(new AddRoomImageDto(
                ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
                ImageExtension: ext,
                ImageName: name,
                Description: desc,
                Category: cat                           // enum value
            ));
        }

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddRoomImagesCommand(roomId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpGet("GetRoom"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> GetRoom([FromQuery] GetRoomsQuery cmd, CancellationToken ct)
     => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("UpdateRoom"), AllowAnonymous]
    public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result ? Ok(new { Message = "Room updated successfully." }) : NotFound("Room not found.");
    }

    [HttpDelete("DeleteRoom"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<string>>> DeleteRoom([FromQuery] DeleteRoomCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("RoomImages/{roomId}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<RoomImageDto>>>> GetRoomImages(Guid roomId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRoomImagesQuery { RoomId = roomId }, ct);
        return Ok(result);
    }

    [HttpPost("AddRoom-Services"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> AddRoomServices(AssignServicesToRoomCommand cmd, CancellationToken ct)
       => Ok(await _mediator.Send(cmd, ct));


    [HttpGet("rooms/{roomId:guid}/with-services"), AllowAnonymous]
    public async Task<IActionResult> GetRoomWithServices(Guid roomId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRoomWithServiceSelectionsQuery { RoomId = roomId }, ct);
        return StatusCode(result.Status, result);
    }

    [HttpPost("AddRoom-Charges"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> AddRoomCharge([FromBody] AddRoomCharges cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));
}

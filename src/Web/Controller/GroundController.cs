using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.GroundReservations.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Queries;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundStandardTime.Command;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundStandardTime.Queries;
using DHAFacilitationAPIs.Application.Feature.Grounds.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomCharges;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.CreateRoom;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.UpdateRoom;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.UpdateRoomCharges;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetImageCategories;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomCharges;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomImages;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Ground")]
public class GroundController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _files;
    public GroundController(IMediator mediator, IFileStorageService files)
    {
        _mediator = mediator;
        _files = files;
    }

    [HttpPost("Create-Ground"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(RegisterGroundCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("image-categories"), AllowAnonymous]
    public async Task<IActionResult> GetImageCategories(CancellationToken ct)
         => Ok(await _mediator.Send(new GetGroundImageCategoriesQuery(), ct));

    [HttpPost("{GroundId:guid}/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddRoomImages(
    Guid GroundId,
    [FromForm] AddGroundImageDTO form,
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

        var folder = $"Ground/{GroundId}";
        var uploaded = new List<GroundImagesRecord>();

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

            uploaded.Add(new GroundImagesRecord(
                ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
                ImageExtension: ext,
                ImageName: name,
                Description: desc,
                Category: cat                           // enum value
            ));
        }

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddGroundImagesCommand(GroundId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }


    [HttpGet("GetGround"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<GroundDTO>>> GetRoom([FromQuery] GetGroundQuery cmd, CancellationToken ct)
     => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetGroundDetail"), AllowAnonymous]
    public async Task<IActionResult> GetRoomDetails([FromQuery] Guid groundID, [FromQuery] GroundCategory GroundCategory, DateOnly bookingDate)
    {
        var result = await _mediator.Send(new GetGroundQueryById(groundID, GroundCategory,bookingDate));
        return Ok(result);
    }

    [HttpPut("UpdateGround"), AllowAnonymous]
    public async Task<IActionResult> UpdateRoom([FromBody] UpdateGroundCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result ? Ok(new { Message = "Room updated successfully." }) : NotFound("Room not found.");
    }

    [HttpGet("GroundImages/{GroundId}"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<GroundImagesDTO>>>> GetRoomImages(Guid GroundId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGroundImagesQuery { GroundId = GroundId }, ct);
        return Ok(result);
    }

    [HttpPost("CreateGround-Slots"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddRoomCharge([FromBody] AddGroundSlotCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetGround-Slots"), AllowAnonymous]
    public async Task<IActionResult> GetRoomCharges([FromQuery] Guid roomId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGroundSlotsQuery { GroundId = roomId}, ct);
        return Ok(new { charges = result });
    }
    [HttpPut("UpdateGround-Slots"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateRoomCharge([FromBody] UpdateGroundSlotsCommand cmd, CancellationToken ct)
    => Ok(await _mediator.Send(cmd, ct));


    [HttpPost("AddGround-StandardTime"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateGroundStandardTime([FromBody] CreateGroundBookingStandardTimeCommand cmd, CancellationToken ct)
    => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetGround-StandardTime"), AllowAnonymous]
    public async Task<IActionResult> GetGroundStandardTime([FromQuery] Guid GroundId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGroundBookingStandardTimeQuery(GroundId), ct);
        return Ok(new { charges = result });
    }

    [HttpPut("UpdateGround-StandardTime"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateStandardTime([FromBody] UpdateGroundStandardTimeCommand cmd, CancellationToken ct)
   => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Confirm-Booking"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddRoomCharge([FromBody] GroundComfimBookingCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPost("Booking-CheckIn/CheckOut"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<List<Guid>>>> AddRoomCharge([FromBody] CheckInCheckOutCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

}

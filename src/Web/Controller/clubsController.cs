using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.AddFacilityToClub;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.CreateFacility;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubImages.Command;
using DHAFacilitationAPIs.Application.Feature.CBMS.Discount.Commands.AssignFacilityUnitDiscount;
using DHAFacilitationAPIs.Application.Feature.CBMS.Discount.Commands.CreateDiscount;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityImages.Commands;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.CreateFacilityService;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Commands.CreateFacilityUnit;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.CreateFacilityUnitBookingConfig;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.AddFacilityUnitService;
using DHAFacilitationAPIs.Application.Feature.CBMS.Tax.Commands.AssignFacilityUnitTax;
using DHAFacilitationAPIs.Application.Feature.CBMS.Tax.Commands.CreateTax;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClubBookingStandardTimeCommand;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubBookingStandardTimes;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "CBMS")]
public class clubsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _files;
    public clubsController(IMediator mediator,IFileStorageService fileStorage){
        _mediator = mediator;
        _files = fileStorage;
    }

    [HttpPost("Create"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteClubCommand(id), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Club?>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetClubByIdQuery(id), ct));

    [HttpGet]
    public async Task<ActionResult<List<ClubDto>>> GetAll([FromQuery] ClubType clubType)
        => Ok(await _mediator.Send(new GetClubsQuery(clubType)));

    [HttpPost("Create-Club Booking Standard Time"), AllowAnonymous]
    public async Task<IActionResult> CreateClubBookingStandardTime([FromBody] ClubBookingStandardTimeDto dto, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateClubBookingStandardTimeCommand { Dto = dto }, ct);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpPut("Update-Club Booking Standard Time"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateClubBookingStandardTime([FromBody] UpdateClubBookingStandardTimeCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("Delete-Club Booking Standard Time"), AllowAnonymous]
    public async Task<ActionResult<SuccessResponse<Guid>>> DeleteClubBookingStandardTime(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteClubBookingStandardTimeCommand { Id = id }, ct);
        return StatusCode(result.Status, result);
    }

    [HttpGet("Get-Club Booking Standard Time"), AllowAnonymous]
    public async Task<IActionResult> GetClubBookingStandardTimes([FromQuery] Guid? clubId, ClubType clubType, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClubBookingStandardTimesQuery { ClubId = clubId, ClubType = clubType }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("Club/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddClubImages(
    Guid clubId,
    [FromForm] AddClubImagesFlatForm form,
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

        var folder = $"Club/{clubId}";
        var uploaded = new List<AddClubImageDTO>();

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

            uploaded.Add(new AddClubImageDTO(
                ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
                ImageExtension: ext,
                ImageName: name,
                Description: desc,
                Category: cat                           // enum value
            ));
        }

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddClubImagesCommand(clubId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpPost("Create-Club-Category")]
    public async Task<IActionResult> Create(CreateClubCategoryCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    //[HttpGet("GetClubCategory")]
    //public async Task<IActionResult> Get(CancellationToken ct)
    //    => Ok(await _mediator.Send(new GetClubCategoriesQuery(), ct));
    [HttpPut("Club-Category-Update{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ClubUpdateServiceCategoryRequest body,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateClubCategoryCommand(id, body.Name, body.Code,body.DisplayName,body.Descriptio), ct));
    //[HttpPut("Club-Category-Update{id:guid}")]
    //public async Task<IActionResult> Update(
    //    Guid id,
    //    [FromBody] ClubUpdateServiceCategoryRequest body,
    //    CancellationToken ct)
    //    => Ok(await _mediator.Send(
    //        new UpdateClubCategoryCommand(id,body.ClubId, body.Name, body.Code), ct));

    [HttpDelete("Club-Category-Delete{id:guid}")]
    public async Task<IActionResult> ClubDelete(
        Guid id,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new DeleteClubCategoryCommand(id), ct));


    //[HttpPost("Create-Club-Facilities")]
    //public async Task<IActionResult> Create(CreateClubCategoryCommand cmd, CancellationToken ct)
    //    => Ok(await _mediator.Send(cmd, ct));

    //[HttpGet("GetClubCategory")]
    //public async Task<IActionResult> Get(CancellationToken ct)
    //    => Ok(await _mediator.Send(new GetClubCategoriesQuery(), ct));

    //[HttpPut("Club-Category-Update{id:guid}")]
    //public async Task<IActionResult> Update(
    //    Guid id,
    //    [FromBody] ClubUpdateServiceCategoryRequest body,
    //    CancellationToken ct)
    //    => Ok(await _mediator.Send(
    //        new UpdateClubCategoryCommand(id, body.Name, body.Code, body.DisplayName, body.Descriptio), ct));

    //[HttpDelete("Club-Category-Delete{id:guid}")]
    //public async Task<IActionResult> ClubDelete(
    //    Guid id,
    //    CancellationToken ct)
    //    => Ok(await _mediator.Send(
    //        new DeleteClubCategoryCommand(id), ct));
    [HttpPost("CreateFacility")]
    public async Task<IActionResult> CreateFacility(
        [FromBody] CreateFacilityCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("AddFacilityToClub")]
    public async Task<IActionResult> AddFacilityToClub(
        Guid clubId,
        [FromBody] AddFacilityToClubCommand body,
        CancellationToken ct)
    {
        var command = body with { ClubId = clubId };

        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }


    //[HttpPost("Create-Club-Service")]
    //public async Task<IActionResult> Create(CreateClubServiceProcessCommand cmd, CancellationToken ct)
    //   => Ok(await _mediator.Send(cmd, ct));


    //[HttpGet("by-category/{categoryId:guid}")]
    //public async Task<IActionResult> ByCategory(Guid categoryId, CancellationToken ct)
    //    => Ok(await _mediator.Send(new GetProcessesByCategoryQuery(categoryId), ct));

    ////[HttpGet("by-category/{categoryId:guid}")]
    ////public async Task<IActionResult> ByCategory(Guid categoryId, CancellationToken ct)
    ////    => Ok(await _mediator.Send(new GetProcessesByCategoryQuery(categoryId), ct));

    //[HttpGet("by-category/{categoryId:guid}")]
    //public async Task<IActionResult> GetByCategory(
    //    Guid categoryId,
    //    CancellationToken ct)
    //{
    //    return Ok(await _mediator.Send(

    //        new getclubca(categoryId), ct));
    //        new GetClubServiceProcessByCatQuery(categoryId), ct));
    //}

    //[HttpGet("Club-service{processId:guid}")]
    //public async Task<IActionResult> GetServiceById(
    //    Guid processId,
    //    CancellationToken ct)
    //{
    //    return Ok(await _mediator.Send(
    //        new GetClubServiceProvessByIdQuery(processId), ct));
    //}

    //[HttpPut("Update=Club-service{id:guid}")]
    //public async Task<IActionResult> Update(UpdateClubServiceProcess cmd, CancellationToken ct)
    //{
    //    return Ok(await _mediator.Send(cmd, ct));
    //}

    //[HttpDelete("Delete-Club-service{id:guid}")]
    //public async Task<IActionResult> ServiceDelete(
    //    Guid id,
    //    CancellationToken ct)
    //    => Ok(await _mediator.Send(new DeleteClubServiceProcessCommand(id), ct));

    //[HttpPost("{serviceId:guid}/images/add"), AllowAnonymous]
    //[Consumes("multipart/form-data")]
    //[RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    //public async Task<IActionResult> AddRoomImages(
    //Guid serviceId,
    //[FromForm] AddClubImageFormDto form,
    //CancellationToken ct)
    //{
    //    if (!ModelState.IsValid) return ValidationProblem(ModelState);
    //    if (form.Files == null || form.Files.Count == 0)
    //        return BadRequest("No images uploaded.");

    //    // Keep parallel arrays aligned (if provided)
    //    int n = form.Files.Count;
    //    if (form.ImageNames.Count != 0 && form.ImageNames.Count != n)
    //        return BadRequest("ImageNames count must match Files count.");
    //    if (form.Descriptions.Count != 0 && form.Descriptions.Count != n)
    //        return BadRequest("Descriptions count must match Files count.");
    //    if (form.Categories.Count != 0 && form.Categories.Count != n)
    //        return BadRequest("Categories count must match Files count.");

    //    var folder = $"ClubServices/{serviceId}";
    //    var uploaded = new List<AddClubServiceImageDto>();

    //    for (int i = 0; i < n; i++)
    //    {
    //        var file = form.Files[i];

    //        // Save file (your IFileStorageService validates size/ext & creates folder)
    //        //var relativePath = await _files.SaveFileAsync(file, folder, ct);
    //        var relativePath = await _files.SaveFileAsync(
    //          file,
    //          folder,
    //          ct,
    //          maxBytes: 5 * 1024 * 1024,                       // 5 MB
    //          allowedExtensions: new[] { ".jpg", ".jpeg", ".png" }
    //         );

    //        var ext = Path.GetExtension(relativePath);

    //        // Pick metadata by index (or defaults)
    //        var name = form.ImageNames.ElementAtOrDefault(i);
    //        var desc = form.Descriptions.ElementAtOrDefault(i);
    //        var cat = form.Categories.ElementAtOrDefault(i);

    //        uploaded.Add(new AddClubServiceImageDto(
    //            ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
    //            ImageExtension: ext,
    //            ImageName: name,
    //            Description: desc,
    //            Category: cat                           // enum value
    //        ));
    //    }

    //    // Hand off to your command (enforces “only one Main” etc.)
    //    var cmd = new AddClubServiceImageCommand(serviceId, uploaded);
    //    var result = await _mediator.Send(cmd, ct);
    //    return Ok(result);
    //}



    [HttpPost("Facility/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddFacilityImages(
    Guid FacilityId,
    [FromForm] AddClubImagesFlatForm form,
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

        var folder = $"Facility/{FacilityId}";
        var uploaded = new List<AddClubImageDTO>();

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

            uploaded.Add(new AddClubImageDTO(
                ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
                ImageExtension: ext,
                ImageName: name,
                Description: desc,
                Category: cat                           // enum value
            ));
        }

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddFacilityImagesCommand(FacilityId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpPost("services")]
    public Task<ApiResult<Guid>> AddService(CreateFacilityServiceDto dto)
        => _mediator.Send(new CreateFacilityServiceCommand(dto));

    [HttpPost("units")]
    public Task<ApiResult<Guid>> AddUnit(CreateFacilityUnitDto dto)
        => _mediator.Send(new CreateFacilityUnitCommand(dto));

    [HttpPost("unit-config")]
    public Task<ApiResult<Guid>> UpsertUnitConfig(CreateFacilityUnitBookingConfigDto dto)
        => _mediator.Send(new CreateFacilityUnitBookingConfigCommand(dto));

    [HttpPost("unit-services")]
    public Task<ApiResult<Guid>> AddUnitService(AddFacilityUnitServiceDto dto)
        => _mediator.Send(new AddFacilityUnitServiceCommand(dto));


    [HttpPost("FacilityUnit/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    public async Task<IActionResult> AddFacilityUnitImages(
    Guid FacilityUnitId,
    [FromForm] AddClubImagesFlatForm form,
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

        var folder = $"FacilityUnit/{FacilityUnitId}";
        var uploaded = new List<AddClubImageDTO>();

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

            uploaded.Add(new AddClubImageDTO(
                ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
                ImageExtension: ext,
                ImageName: name,
                Description: desc,
                Category: cat                           // enum value
            ));
        }

        // Hand off to your command (enforces “only one Main” etc.)
        var cmd = new AddFacilityImagesCommand(FacilityUnitId, uploaded);
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }


    [HttpPost("discounts")]
    public Task<ApiResult<Guid>> CreateDiscount(CreateDiscountDto dto)
        => _mediator.Send(new CreateDiscountCommand(dto));

    [HttpPost("taxes")]
    public Task<ApiResult<Guid>> CreateTax(CreateTaxDto dto)
        => _mediator.Send(new CreateTaxCommand(dto));

    [HttpPost("facility-unit/discount")]
    public Task<ApiResult<Guid>> AssignDiscount(AssignFacilityUnitDiscountDto dto)
        => _mediator.Send(new AssignFacilityUnitDiscountCommand(dto));

    [HttpPost("facility-unit/tax")]
    public Task<ApiResult<Guid>> AssignTax(AssignFacilityUnitTaxDto dto)
        => _mediator.Send(new AssignFacilityUnitTaxCommand(dto));
}
public record ClubUpdateServiceCategoryRequest(
    Guid ClubId,
   string Name,
   string Code,
   string DisplayName,
   string Descriptio
);

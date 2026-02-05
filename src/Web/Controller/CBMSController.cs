using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries.GetClubCategories;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Command;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.ActiveINaActiveFacilityClub;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.AddFacilityToClub;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.CreateFacility;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.DeleteFacilityToClub;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.UpdateFacilityToClub;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubImages.Command;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityImages.Commands;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetAllClubs;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "CBMS")]
public class CBMSController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _files;
    public CBMSController(IMediator mediator,IFileStorageService fileStorage){
        _mediator = mediator;
        _files = fileStorage;
    }

    #region Main Club Crud here

    [HttpPost("Create")]
    [Tags("Clubs")]
    public async Task<ActionResult<SuccessResponse<Guid>>> Create(CreateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet]
    [Tags("Clubs")]
    public async Task<ActionResult> GetAllClubs(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllClubsDetailQuery(),ct));

    [HttpGet("{id:guid}")]
    [Tags("Clubs")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetClubDetailById(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResponse<string>>> Update(Guid id, UpdateClubCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Tags("Clubs")]
    public async Task<ActionResult<SuccessResponse<string>>> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteClubCommand(id), ct));

    [HttpPost("Club/images/add")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    [Tags("Clubs")]
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

    #endregion

    #region Category here
    [HttpPost("Category")]
    [Tags("Category")]
    public async Task<IActionResult> Create(CreateClubCategoryCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpGet("GetCategory")]
    [Tags("Category")]
    public async Task<ActionResult<List<ClubCategoriesDTO>>> GetCategory()
    {
        var list = await _mediator.Send(new GetClubWiseCategories());
        return Ok(list);
    }

    [HttpGet("GetClubCategories/{ClubId:guid}")]
    [Tags("Category")]
    public async Task<ActionResult<ClubCategoriesDTO>> GetClubCategories(Guid ClubId)
    {
        var list = await _mediator.Send(new GetClubCategoriesQuery(ClubId));
        return Ok(list);
    }

    [HttpPut("Category/{id:guid}")]
    [Tags("Category")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ClubUpdateServiceCategoryRequest body,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateClubCategoryCommand(id, body.Name, body.Code,body.DisplayName,body.Descriptio), ct));

    [HttpDelete("Category/{id:guid}")]
    [Tags("Category")]
    public async Task<IActionResult> ClubDelete(
        Guid id,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new DeleteClubCategoryCommand(id), ct));

   
    #endregion


    #region Facility Crud here
    [HttpPost("CreateFacility")]
    [Tags("Facility")]
    public async Task<IActionResult> CreateFacility(
        [FromBody] CreateFacilityCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
   
    [HttpGet("GetFacilities")]
    [Tags("Facility")]
    public async Task<ActionResult<List<FacilitiesDTO>>> GetFacilities()
    {
        var list = await _mediator.Send(new GetAllFacilitiesQuery());
        return Ok(list);
    }

    [HttpGet("GetFacilityById/{Id:guid}")]
    [Tags("Facility")]
    public async Task<ActionResult<FacilitiesDTO>> GetFacilityBYId(Guid Id)
    {
        var list = await _mediator.Send(new GetFacilityByIdQuery(Id));
        return Ok(list);
    }

    [HttpDelete("DeleteFacility/{Id:guid}")]
    [Tags("Facility")]
    public async Task<IActionResult> DeleteFacility(
       Guid Id,
       CancellationToken ct)
       => Ok(await _mediator.Send(
           new DeleteFaciltityCommand(Id), ct));


    [HttpGet("GetFacilityByCategoryId/{CategoryId:guid}")]
    [Tags("Facility")]
    public async Task<ActionResult<List<FacilitiesDTO>>> GetAllFacilityByCat(Guid CategoryId)
    {
        var list = await _mediator.Send(new GetFacilityByCategoryQuery(CategoryId));
        return Ok(list);
    }

   
    #endregion

    [HttpPost("AddFacilityToClub")]
    [Tags("ClubFacilities")]
    public async Task<IActionResult> AddFacilityToClub(
        Guid ClubId,
        [FromBody] AddFacilityToClubCommand body,
        CancellationToken ct)
    {
        var command = body with { ClubId = ClubId };

        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("GetAllClubFacilties")]
    [Tags("ClubFacilities")]
    public async Task<ActionResult<List<ClubFacilitiesDTO>>> GetAllClubFacilties()
    {
        var list = await _mediator.Send(new GetAllClubFaciltiesQuery());
        return Ok(list);
    }

    [HttpGet("GetAllClubFaciltiesByClubId/{ClubId:guid}")]
    [Tags("ClubFacilities")]
    public async Task<ActionResult<ClubFacilitiesDTO>> GetFacilityToClub(Guid ClubId)
    {
        var list = await _mediator.Send(new GetClubFacilitiesByClubIdQuery(ClubId));
        return Ok(list);
    }
    
 
    [HttpPut("UpdateClubFacility")]
    [Tags("ClubFacilities")]
    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateClubFacility([FromBody] UpdateFacilityToClubCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpDelete("DeleteClubFacility")]
    [Tags("ClubFacilities")]
    public async Task<IActionResult> DeleteClubFacility(
       Guid id,
       CancellationToken ct)
       => Ok(await _mediator.Send(
           new DeleteFacilityClubCommand(id), ct));

    [HttpPost("Facility/images/add"), AllowAnonymous]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    [Tags("ClubFacilities")]
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

    //    [HttpPost("AddFacilityService")]
    //    [Tags("FacilityService")]
    //    public Task<ApiResult<Guid>> AddService(CreateFacilityServiceDto dto)
    //        => _mediator.Send(new CreateFacilityServiceCommand(dto));

    //    [HttpPut("UpdateFacilityService")]
    //    public async Task<ActionResult<SuccessResponse<bool>>> Updateservices([FromBody] UpdateFacilityServiceCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpDelete("DeleteFacilityService")]
    //    public async Task<IActionResult> Facilityservices(
    //       Guid id,
    //       CancellationToken ct)
    //       => Ok(await _mediator.Send(
    //           new DeleteFacilityServiceCommand(id), ct));

    //    [HttpPut("Active/InAtive-services"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> Updateservices(bool Active, ActiveinActiveFacilityServiceCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpGet("Get-services")]
    //    public async Task<ActionResult<List<ClubFacilitiesDTO>>> Getservices()
    //    {
    //        var list = await _mediator.Send(new GetAllFacilityServiceQuery());
    //        return Ok(list);
    //    }

    //    [HttpGet("Get-services-id")]
    //    public async Task<ActionResult<ClubFacilitiesDTO>> GetservicesById(Guid Id)
    //    {
    //        var list = await _mediator.Send(new GetFacilityServiceByIdQuery(Id));
    //        return Ok(list);
    //    }
    //    [HttpGet("Get-services-By-Fac")]
    //    public async Task<ActionResult<List<ClubFacilitiesDTO>>> GetservicesFac(Guid Facility)
    //    {
    //        var list = await _mediator.Send(new GetFacilityServiceByFacilityQuery(Facility));
    //        return Ok(list);
    //    }


    //    [HttpPost("units")]
    //    public Task<ApiResult<Guid>> AddUnit(CreateFacilityUnitDto dto)
    //        => _mediator.Send(new CreateFacilityUnitCommand(dto));

    //    [HttpPut("Update-units"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> Updateunits([FromBody] UpdateFacilityUnitCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpDelete("Delete-units")]
    //    public async Task<IActionResult> Facilityunits(
    //       Guid id,
    //       CancellationToken ct)
    //       => Ok(await _mediator.Send(
    //           new DeleteFacilityUnitCommand(id), ct));

    //    [HttpPut("Active/InAtive-units"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateunitsConfig(bool Active, ActiveInActiveFacilityUnitCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }
    //    [HttpGet("Get-units")]
    //    public async Task<ActionResult<List<FacilityUnitDTO>>> Getunits()
    //    {
    //        var list = await _mediator.Send(new GetAllFaciltiUnitQuery());
    //        return Ok(list);
    //    }

    //    [HttpGet("Get-units-id")]
    //    public async Task<ActionResult<FacilityUnitDTO>> GetunitsById(Guid Id)
    //    {
    //        var list = await _mediator.Send(new GetFacilityUnitByIdQuery(Id));
    //        return Ok(list);
    //    }
    //    [HttpGet("Get-units-By-Fac")]
    //    public async Task<ActionResult<List<FacilityUnitDTO>>> GetunitsFac(Guid? ClubId,Guid? Facility)
    //    {
    //        var list = await _mediator.Send(new GetFacilityUniByClubFacQuery(ClubId, Facility));
    //        return Ok(list);
    //    }



    //    [HttpPost("unit-config")]
    //    public async Task<ActionResult<ApiResult<Guid>>> UpsertUnitConfig(
    //    [FromBody] CreateFacilityUnitBookingConfigCommand cmd,
    //    CancellationToken ct
    //)
    //    => Ok(await _mediator.Send(cmd, ct));




    //    [HttpPut("Update-unit-config"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> UpdateunitsConfig([FromBody] UpdateFacilityUnitBookingConfigCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpDelete("Delete-unit-config")]
    //    public async Task<IActionResult> FacilityunitsConfig(
    //       Guid id,
    //       CancellationToken ct)
    //       => Ok(await _mediator.Send(
    //           new DeleteFacilityUnitBookingConfigCommand(id), ct));

    //    [HttpPut("Active/InAtive-unit-config"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> Updateunits(bool Active, ActiveInActiveFacilityUnitBookingConfigCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpGet("Get-units-config")]
    //    public async Task<ActionResult<List<ClubFacilitiesDTO>>> Getunitsconfig()
    //    {
    //        var list = await _mediator.Send(new GetAllFacilityUnitBookingConfigQuery());
    //        return Ok(list);
    //    }

    //    [HttpGet("Get-units-config-id")]
    //    public async Task<ActionResult<ClubFacilitiesDTO>> GetunitsconfigById(Guid Id)
    //    {
    //        var list = await _mediator.Send(new GetFacilityUnitConfigByIdQuery(Id));
    //        return Ok(list);
    //    }



    //    [HttpPost("unit-services")]
    //    public Task<ApiResult<Guid>> AddUnitService(AddFacilityUnitServiceDto dto)
    //        => _mediator.Send(new AddFacilityUnitServiceCommand(dto));

    //    [HttpPut("Update-unit-services"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> Updateunitsservices([FromBody] UpdateFacilityUnitServiceCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpDelete("Delete-unit-services")]
    //    public async Task<IActionResult> Facilityunitsservices(
    //       Guid id,
    //       CancellationToken ct)
    //       => Ok(await _mediator.Send(
    //           new DeleteFacilityUnitBookingConfigCommand(id), ct));

    //    [HttpPut("Active/InAtive-unit-services"), AllowAnonymous]
    //    public async Task<ActionResult<SuccessResponse<Guid>>> Updateservices(bool Active, ActiveInActiveFaciltityServiceUnitCommand cmd, CancellationToken ct)
    //    {
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }

    //    [HttpGet("Get-unit-services")]
    //    public async Task<ActionResult<List<FacilityServiceUnitDTO>>> Getunitservice()
    //    {
    //        var list = await _mediator.Send(new GetAllFacilityServiceUnitQuery());
    //        return Ok(list);
    //    }

    //    [HttpGet("Get-unit-services-id")]
    //    public async Task<ActionResult<FacilityServiceUnitDTO>> GetunitserviceById(Guid Id)
    //    {
    //        var list = await _mediator.Send(new GetFacilityServiceUnitByIdQuery(Id));
    //        return Ok(list);
    //    }


    //    [HttpPost("FacilityUnit/images/add"), AllowAnonymous]
    //    [Consumes("multipart/form-data")]
    //    [RequestSizeLimit(50_000_000)] // optional: 50 MB cap
    //    public async Task<IActionResult> AddFacilityUnitImages(
    //    Guid FacilityUnitId,
    //    [FromForm] AddClubImagesFlatForm form,
    //    CancellationToken ct)
    //    {
    //        if (!ModelState.IsValid) return ValidationProblem(ModelState);
    //        if (form.Files == null || form.Files.Count == 0)
    //            return BadRequest("No images uploaded.");

    //        // Keep parallel arrays aligned (if provided)
    //        int n = form.Files.Count;
    //        if (form.ImageNames.Count != 0 && form.ImageNames.Count != n)
    //            return BadRequest("ImageNames count must match Files count.");
    //        if (form.Descriptions.Count != 0 && form.Descriptions.Count != n)
    //            return BadRequest("Descriptions count must match Files count.");
    //        if (form.Categories.Count != 0 && form.Categories.Count != n)
    //            return BadRequest("Categories count must match Files count.");

    //        var folder = $"FacilityUnit/{FacilityUnitId}";
    //        var uploaded = new List<AddClubImageDTO>();

    //        for (int i = 0; i < n; i++)
    //        {
    //            var file = form.Files[i];

    //            // Save file (your IFileStorageService validates size/ext & creates folder)
    //            //var relativePath = await _files.SaveFileAsync(file, folder, ct);
    //            var relativePath = await _files.SaveFileAsync(
    //              file,
    //              folder,
    //              ct,
    //              maxBytes: 5 * 1024 * 1024,                       // 5 MB
    //              allowedExtensions: new[] { ".jpg", ".jpeg", ".png" }
    //             );

    //            var ext = Path.GetExtension(relativePath);

    //            // Pick metadata by index (or defaults)
    //            var name = form.ImageNames.ElementAtOrDefault(i);
    //            var desc = form.Descriptions.ElementAtOrDefault(i);
    //            var cat = form.Categories.ElementAtOrDefault(i);

    //            uploaded.Add(new AddClubImageDTO(
    //                ImageURL: relativePath,                 // e.g. /uploads/rooms/{roomId}/{guid}.jpg
    //                ImageExtension: ext,
    //                ImageName: name,
    //                Description: desc,
    //                Category: cat                           // enum value
    //            ));
    //        }

    //        // Hand off to your command (enforces “only one Main” etc.)
    //        var cmd = new AddFacilityUnitImagesCommand(FacilityUnitId, uploaded);
    //        var result = await _mediator.Send(cmd, ct);
    //        return Ok(result);
    //    }


    //    [HttpPost("discounts")]
    //    public Task<ApiResult<Guid>> CreateDiscount(CreateDiscountDto dto)
    //        => _mediator.Send(new CreateDiscountCommand(dto));

    //    [HttpPost("taxes")]
    //    public Task<ApiResult<Guid>> CreateTax(CreateTaxDto dto)
    //        => _mediator.Send(new CreateTaxCommand(dto));

    //    [HttpPost("facility-unit/discount")]
    //    public Task<ApiResult<Guid>> AssignDiscount(AssignFacilityUnitDiscountDto dto)
    //        => _mediator.Send(new AssignFacilityUnitDiscountCommand(dto));

    //    [HttpPost("facility-unit/tax")]
    //    public Task<ApiResult<Guid>> AssignTax(AssignFacilityUnitTaxDto dto)
    //        => _mediator.Send(new AssignFacilityUnitTaxCommand(dto));


    //[HttpGet("GetClubFacility")]
    //public async Task<ActionResult<List<ClubFacilitiesDTO>>> GetAllFacilityToClubByClubFac(Guid ClubId, Guid Facility)
    //{
    //    var list = await _mediator.Send(new GetClubFacilityByClubFacQuery(ClubId, Facility));
    //    return Ok(list);
    //}

    //[HttpPut("Active/InAtive-FacilityToClub"), AllowAnonymous]
    //public async Task<ActionResult<SuccessResponse<Guid>>> UpdateFacilityToClub(bool Active, ActiveInActiveFacilityClubCommand cmd, CancellationToken ct)
    //{
    //    var result = await _mediator.Send(cmd, ct);
    //    return Ok(result);
    //}

    //[HttpPut("Active/InAtive-Facility")]
    //[Tags("Facility")]
    //public async Task<ActionResult<SuccessResponse<Guid>>> UpdateFacility(bool Active, ActiveInActiveFaciltityCommand cmd, CancellationToken ct)
    //{
    //    var result = await _mediator.Send(cmd, ct);
    //    return Ok(result);
    //}




}
public record ClubUpdateServiceCategoryRequest(
    Guid ClubId,
   string Name,
   string Code,
   string DisplayName,
   string Descriptio
);

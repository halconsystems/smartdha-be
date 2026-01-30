//using DHAFacilitationAPIs.Application.Common.Interfaces;
//using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;
//using DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Command;
//using DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;
//using DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisite.Command;
//using DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisite.Queries;
//using DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisiteDefination.Command;
//using DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisiteDefination.Queries;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.AddProcessPrerequisite;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreateAndAttachPrerequisite;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.SoftDeleteProcessPrerequisite;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.UpdatePrerequisiteDefinition;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetAllPrerequisites;
//using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;
//using DHAFacilitationAPIs.Domain.Enums.PMS;
//using Microsoft.AspNetCore.Mvc;

//namespace DHAFacilitationAPIs.Web.Controller;

//[Route("api/[controller]")]
//[ApiController]
//[ApiExplorerSettings(GroupName = "CBMS")]

//public class ClubServicePreRequisiteController : BaseApiController
//{
//    private readonly IMediator _mediator;
//    private readonly IFileStorageService _files;
//    public ClubServicePreRequisiteController(IMediator mediator, IFileStorageService fileStorage)
//    {
//        _mediator = mediator;
//        _files = fileStorage;
//    }

//    //[HttpPost("Create-Club-Category")]
//    //public async Task<IActionResult> Create(CreateClubCategoryCommand cmd, CancellationToken ct)
//    //   => Ok(await _mediator.Send(cmd, ct));

//    //[HttpPost("Create-Club-Service")]
//    //public async Task<IActionResult> Create(CreateClubServiceProcessCommand cmd, CancellationToken ct)
//    //  => Ok(await _mediator.Send(cmd, ct));

//    //[HttpGet("by-category/{categoryId:guid}")]
//    //public async Task<IActionResult> GetByCategory(
//    //   Guid categoryId,
//    //   CancellationToken ct)
//    //{
//    //    return Ok(await _mediator.Send(
//    //        new GetClubServiceProcessByCatQuery(categoryId), ct));
//    //}

//    //[HttpGet("Club-service{processId:guid}")]
//    //public async Task<IActionResult> GetServiceById(
//    //   Guid processId,
//    //   CancellationToken ct)
//    //{
//    //    return Ok(await _mediator.Send(
//    //        new GetClubServiceProvessByIdQuery(processId), ct));
//    //}

//    [HttpPost]
//    public async Task<IActionResult> CreateAndAttach(
//       CreateClubPrerequisiteDefinitionCommand cmd,
//       CancellationToken ct)
//       => Ok(await _mediator.Send(cmd, ct));

//    [HttpGet]
//    public async Task<IActionResult> GetAll(CancellationToken ct)
//        => Ok(await _mediator.Send(new GetAllPreRequisiteDefinationQuery(), ct));

//    [HttpGet("{processId:guid}/prerequisites")]
//    public async Task<IActionResult> GetPrerequisites(
//       Guid processId,
//       CancellationToken ct)
//    {
//        var result = await _mediator.Send(
//            new GetClubProcessAllPrerequisiteQuery(processId), ct);

//        return Ok(result);
//    }

//    [HttpPut("definitions/{id:guid}")]
//    public async Task<IActionResult> Update(
//    Guid id,
//    [FromBody] UpdateClubPrerequisiteDefinitionCommandBody body,
//    CancellationToken ct)
//    => Ok(await _mediator.Send(new UpdateClubPrerequisiteDefinitionCommand(
//        id,
//        body.Name,
//        body.Code,
//        body.Type,
//        body.MinLength,
//        body.MaxLength,
//        body.AllowedExtensions,
//        body.Options
//    ), ct));

//    [HttpDelete("ProcessPrerequisite/{id:guid}")]
//    public async Task<IActionResult> ProcessPrerequisite(
//       Guid id,
//       CancellationToken ct)
//    {
//        return Ok(await _mediator.Send(
//            new SoftDeleteClubProcessPrerequisiteCommand(id), ct));
//    }

//    [HttpPost("attach")]
//    public async Task<IActionResult> Attach(AddClubProcessPrerequisiteCommand cmd, CancellationToken ct)
//        => Ok(await _mediator.Send(cmd, ct));

//    [HttpPost("definitions")]
//    public async Task<IActionResult> CreateDefinition(AddClubProcessDefinationCommand cmd, CancellationToken ct)
//       => Ok(await _mediator.Send(cmd, ct));

//}
//public class UpdateClubPrerequisiteDefinitionCommandBody
//{
//    public string Name { get; set; } = default!;
//    public string Code { get; set; } = default!;
//    public PrerequisiteType Type { get; set; }
//    public int? MinLength { get; set; }
//    public int? MaxLength { get; set; }
//    public string? AllowedExtensions { get; set; }

//    // Default to empty list (never null)
//    public List<PrerequisiteOptionInput>? Options { get; set; } = new();
//}

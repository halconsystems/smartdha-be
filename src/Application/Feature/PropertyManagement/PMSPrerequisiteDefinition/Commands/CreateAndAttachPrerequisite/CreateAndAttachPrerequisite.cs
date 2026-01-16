using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreateAndAttachPrerequisite;
public record CreateAndAttachPrerequisiteCommand
(
    Guid ProcessId,

    // Definition data
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,

    // Process mapping
    bool IsRequired,
    int RequiredByStepNo
) : IRequest<ApiResult<Guid>>;


public class CreateAndAttachPrerequisiteHandler
    : IRequestHandler<CreateAndAttachPrerequisiteCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreateAndAttachPrerequisiteHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateAndAttachPrerequisiteCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Validate process
        var process = await _db.Set<ServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == r.ProcessId, ct);

        if (process == null)
            return ApiResult<Guid>.Fail("Process not found.");

        // 2️⃣ Get or create PrerequisiteDefinition
        var definition = await _db.Set<PrerequisiteDefinition>()
            .FirstOrDefaultAsync(x => x.Code == r.Code, ct);

        if (definition == null)
        {
            definition = new PrerequisiteDefinition
            {
                Name = r.Name.Trim(),
                Code = r.Code.Trim().ToUpperInvariant(),
                Type = r.Type,
                MinLength = r.MinLength,
                MaxLength = r.MaxLength,
                AllowedExtensions = r.AllowedExtensions
            };

            _db.Set<PrerequisiteDefinition>().Add(definition);
            await _db.SaveChangesAsync(ct); // ensure Id generated
        }

        // 3️⃣ Check duplicate attachment
        var alreadyAttached = await _db.Set<ProcessPrerequisite>()
            .AnyAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.PrerequisiteDefinitionId == definition.Id,
                ct);

        if (alreadyAttached)
            return ApiResult<Guid>.Fail("Prerequisite already attached to this process.");

        // 4️⃣ Attach to process
        var mapping = new ProcessPrerequisite
        {
            ProcessId = r.ProcessId,
            PrerequisiteDefinitionId = definition.Id,
            IsRequired = r.IsRequired,
            RequiredByStepNo = r.RequiredByStepNo
        };

        _db.Set<ProcessPrerequisite>().Add(mapping);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(mapping.Id, "Prerequisite created and attached.");
    }
}


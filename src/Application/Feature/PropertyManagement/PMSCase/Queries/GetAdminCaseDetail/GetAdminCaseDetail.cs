using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetAdminCaseDetail;

public class AdminCasePrerequisiteDto
{
    public Guid PrerequisiteDefinitionId { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public PrerequisiteType Type { get; set; }

    public bool IsRequired { get; set; }
    public int RequiredByStepNo { get; set; }

    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateTime? ValueDate { get; set; }
    public bool? ValueBool { get; set; }
}
public class AdminCaseDocumentDto
{
    public Guid DocumentId { get; set; }
    public Guid? PrerequisiteDefinitionId { get; set; }

    public string FileName { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }
}

public class AdminCaseDetailDto
{
    public Guid CaseId { get; set; }
    public string CaseNo { get; set; } = default!;

    public string ProcessName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;

    public CaseStatus Status { get; set; }

    public List<AdminCasePrerequisiteDto> Prerequisites { get; set; } = new();
    public List<AdminCaseDocumentDto> Documents { get; set; } = new();
}



public record GetAdminCaseDetailQuery(Guid CaseId)
    : IRequest<ApiResult<AdminCaseDetailDto>>;

public class GetAdminCaseDetailHandler
    : IRequestHandler<GetAdminCaseDetailQuery, ApiResult<AdminCaseDetailDto>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetAdminCaseDetailHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<AdminCaseDetailDto>> Handle(
        GetAdminCaseDetailQuery request,
        CancellationToken ct)
    {
        var c = await _db.Set<PropertyCase>()
            .AsNoTracking()
            .Where(x => x.Id == request.CaseId)
            .Select(x => new
            {
                x.Id,
                x.CaseNo,
                x.Status,
                ProcessName = x.Process.Name,
                CategoryName = x.Process.Category.Name,
                x.ProcessId
            })
            .FirstOrDefaultAsync(ct);

        if (c == null)
            return ApiResult<AdminCaseDetailDto>.Fail("Case not found.");

        // 1️⃣ Prerequisite definitions + values
        var prereqs = await (
            from pp in _db.Set<ProcessPrerequisite>()
            join pd in _db.Set<PrerequisiteDefinition>()
                on pp.PrerequisiteDefinitionId equals pd.Id
            join pv in _db.Set<CasePrerequisiteValue>()
                .Where(v => v.CaseId == request.CaseId)
                on pd.Id equals pv.PrerequisiteDefinitionId into pvj
            from pv in pvj.DefaultIfEmpty()
            where pp.ProcessId == c.ProcessId
            orderby pp.RequiredByStepNo
            select new AdminCasePrerequisiteDto
            {
                PrerequisiteDefinitionId = pd.Id,
                Name = pd.Name,
                Code = pd.Code,
                Type = pd.Type,
                IsRequired = pp.IsRequired,
                RequiredByStepNo = pp.RequiredByStepNo,
                ValueText = pv.ValueText,
                ValueNumber = pv.ValueNumber,
                ValueDate = pv.ValueDate,
                ValueBool = pv.ValueBool
            }
        ).ToListAsync(ct);

        // 2️⃣ Documents
        var docs = await _db.Set<CaseDocument>()
            .AsNoTracking()
            .Where(x => x.CaseId == request.CaseId)
            .Select(x => new AdminCaseDocumentDto
            {
                DocumentId = x.Id,
                PrerequisiteDefinitionId = x.PrerequisiteDefinitionId,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                ContentType = x.ContentType!,
                FileSize = x.FileSize ?? 0,
                UploadedAt = x.Created
            })
            .ToListAsync(ct);

        var result = new AdminCaseDetailDto
        {
            CaseId = c.Id,
            CaseNo = c.CaseNo,
            ProcessName = c.ProcessName,
            CategoryName = c.CategoryName,
            Status = c.Status,
            Prerequisites = prereqs,
            Documents = docs
        };

        return ApiResult<AdminCaseDetailDto>.Ok(result);
    }
}


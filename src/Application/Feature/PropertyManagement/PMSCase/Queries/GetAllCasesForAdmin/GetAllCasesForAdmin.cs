using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetAllCasesForAdmin;

public class AdminCaseSummaryDto
{
    public Guid CaseId { get; set; }
    public string CaseNo { get; set; } = default!;

    public string ProcessName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;

    public CaseStatus Status { get; set; }

    public string? CurrentStepName { get; set; }
    public string? CurrentDirectorate { get; set; }

    public string ApplicantName { get; set; } = default!;
    public string ApplicantCnic { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}


public record GetAllCasesForAdminQuery()
    : IRequest<ApiResult<List<AdminCaseSummaryDto>>>;
public class GetAllCasesForAdminHandler
    : IRequestHandler<GetAllCasesForAdminQuery, ApiResult<List<AdminCaseSummaryDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IApplicationDbContext _applicationDbContext;

    public GetAllCasesForAdminHandler(IPMSApplicationDbContext db, IApplicationDbContext applicationDbContext)
    {
        _db = db;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<ApiResult<List<AdminCaseSummaryDto>>> Handle(
        GetAllCasesForAdminQuery request,
        CancellationToken ct)
    {

        var cases = await _db.Set<PropertyCase>().AsNoTracking()
            .Where(x => x.IsDeleted != true && x.IsActive == true).
            OrderByDescending(x => x.Created)
            .Select(x => new AdminCaseSummaryDto 
            { 
                CaseId = x.Id, 
                CaseNo = x.CaseNo, 
                ProcessName = x.Process.Name, 
                CategoryName = x.Process.Category.Name, 
                Status = x.Status, 
                CurrentStepName = x.CurrentStep != null ? x.CurrentStep.Name : null, 
                CurrentDirectorate = x.CurrentStep != null ? x.CurrentStep.Directorate.Name : null, 
                ApplicantName = x.ApplicantName ?? "-", 
                ApplicantCnic = x.ApplicantCnic ?? "-", CreatedAt = x.Created 
            }).ToListAsync(ct);

        return ApiResult<List<AdminCaseSummaryDto>>.Ok(cases);
    }
}

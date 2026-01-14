using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
public record CreateCaseCommand(
    Guid UserPropertyId,
    Guid ProcessId,
    string? ApplicantName,
    string? ApplicantCnic,
    string? ApplicantMobile
) : IRequest<ApiResult<Guid>>;

public class CreateCaseHandler : IRequestHandler<CreateCaseCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreateCaseHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateCaseCommand r, CancellationToken ct)
    {
        var propExists = await _db.Set<UserProperty>().AnyAsync(x => x.Id == r.UserPropertyId, ct);
        if (!propExists) return ApiResult<Guid>.Fail("UserProperty not found.");

        var processExists = await _db.Set<ServiceProcess>().AnyAsync(x => x.Id == r.ProcessId, ct);
        if (!processExists) return ApiResult<Guid>.Fail("Process not found.");

        // simple CaseNo generation (replace with your own)
        var caseNo = $"PMS-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var entity = new PropertyCase
        {
            UserPropertyId = r.UserPropertyId,
            ProcessId = r.ProcessId,
            Status = CaseStatus.Draft,
            CurrentStepNo = 0,
            CaseNo = caseNo,
            ApplicantName = r.ApplicantName,
            ApplicantCnic = r.ApplicantCnic,
            ApplicantMobile = r.ApplicantMobile
        };

        _db.Set<PropertyCase>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Case created (Draft).");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseDashboard;
public record GetCaseDashboardQuery()
    : IRequest<ApiResult<CaseDashboardDto>>;
public class GetCaseDashboardHandler
    : IRequestHandler<GetCaseDashboardQuery, ApiResult<CaseDashboardDto>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public GetCaseDashboardHandler(
        IPMSApplicationDbContext db,
        IApplicationDbContext appDb,
        ICurrentUserService current)
    {
        _db = db;
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<CaseDashboardDto>> Handle(
        GetCaseDashboardQuery request,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<CaseDashboardDto>.Fail("Unauthorized.");

        var userId = _current.UserId!.ToString();

        // 🔹 User modules
        var myModuleIds = await _appDb.Set<UserModuleAssignment>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive==true && x.IsDeleted !=true)
            .Select(x => x.ModuleId)
            .ToListAsync(ct);

        if (!myModuleIds.Any())
            return ApiResult<CaseDashboardDto>.Ok(new CaseDashboardDto());

        // 🔹 Base query (only visible cases)
        var baseQuery =
            from c in _db.Set<PropertyCase>().AsNoTracking()
            join ps in _db.Set<ProcessStep>().AsNoTracking()
                on c.ProcessId equals ps.ProcessId
            where
                c.IsActive==true &&
                c.IsDeleted !=true &&
                myModuleIds.Contains(ps.Directorate.ModuleId)
            select new
            {
                c.Status,
                c.CurrentStepNo,
                StepNo = ps.StepNo,
                c.Created
            };

        var data = await baseQuery.ToListAsync(ct);

        var now = DateTime.Now;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        static CaseDashboardSummary BuildSummary(IEnumerable<dynamic> items)
        {
            var summary = new CaseDashboardSummary();

            foreach (var x in items)
            {
                var status =
                    x.Status == CaseStatus.Rejected ? CaseStatus.Rejected :
                    x.Status == CaseStatus.Cancelled ? CaseStatus.Cancelled :
                    x.Status == CaseStatus.Approved ? CaseStatus.Approved :
                    x.Status == CaseStatus.Returned ? CaseStatus.Returned :
                    x.CurrentStepNo == x.StepNo ? CaseStatus.Submitted :
                    x.CurrentStepNo < x.StepNo ? CaseStatus.Draft :
                    x.CurrentStepNo > x.StepNo ? CaseStatus.InProgress :
                    CaseStatus.Approved;

                summary.Total++;

                switch (status)
                {
                    case CaseStatus.Draft: summary.Draft++; break;
                    case CaseStatus.Submitted: summary.Submitted++; break;
                    case CaseStatus.InProgress: summary.InProgress++; break;
                    case CaseStatus.Approved: summary.Approved++; break;
                    case CaseStatus.Returned: summary.Returned++; break;
                    case CaseStatus.Rejected: summary.Rejected++; break;
                }
            }

            return summary;
        }

        var dashboard = new CaseDashboardDto
        {
            Overall = BuildSummary(data),
            Today = BuildSummary(data.Where(x => x.Created >= today)),
            ThisWeek = BuildSummary(data.Where(x => x.Created >= weekStart)),
            ThisMonth = BuildSummary(data.Where(x => x.Created >= monthStart))
        };

        return ApiResult<CaseDashboardDto>.Ok(dashboard);
    }
}

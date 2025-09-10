using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Drivers.Queries;
public record GetDriversQuery(Guid? Id) : IRequest<SuccessResponse<List<DriverInfoDto>>>;


// GET
public class GetDriversHandler : IRequestHandler<GetDriversQuery, SuccessResponse<List<DriverInfoDto>>>
{
    private readonly IOLHApplicationDbContext _context;
    public GetDriversHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<DriverInfoDto>>> Handle(GetDriversQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DriverInfos
            .Include(d => d.DriverStatus)
            .Where(d => d.IsDeleted != true)
            .AsQueryable();

        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var driver = await query.FirstOrDefaultAsync(d => d.Id == request.Id.Value, cancellationToken);
            if (driver == null) throw new ArgumentException($"Driver '{request.Id}' not found.");

            return new SuccessResponse<List<DriverInfoDto>>(new List<DriverInfoDto>
            {
                new DriverInfoDto
                {
                    Id = driver.Id,
                    DriverName = driver.DriverName,
                    CNIC = driver.CNIC,
                    MobileNo = driver.MobileNo,
                    Email = driver.Email,
                    Gender = driver.Gender,
                    LicenceNo = driver.LicenceNo,
                    DriverStatusId = driver.DriverStatusId,
                    DriverStatusName = driver.DriverStatus?.Status ?? string.Empty,
                    StatusDate = driver.StatusDate
                }
            });
        }

        var drivers = await query.ToListAsync(cancellationToken);
        var dtoList = drivers.Select(driver => new DriverInfoDto
        {
            Id = driver.Id,
            DriverName = driver.DriverName,
            CNIC = driver.CNIC,
            MobileNo = driver.MobileNo,
            Email = driver.Email,
            Gender = driver.Gender,
            LicenceNo = driver.LicenceNo,
            DriverStatusId = driver.DriverStatusId,
            DriverStatusName = driver.DriverStatus?.Status ?? string.Empty,
            StatusDate = driver.StatusDate
        }).ToList();

        return new SuccessResponse<List<DriverInfoDto>>(dtoList);
    }
}

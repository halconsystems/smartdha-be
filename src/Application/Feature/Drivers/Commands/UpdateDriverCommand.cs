using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Drivers.Commands;

public record UpdateDriverCommand(
    Guid Id,
    string DriverName,
    string CNIC,
    string MobileNo,
    string? Email,
    string Gender,
    string LicenceNo,
    Guid DriverStatusId,
    DateTime StatusDate
) : IRequest<SuccessResponse<string>>;

// UPDATE
public class UpdateDriverHandler : IRequestHandler<UpdateDriverCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public UpdateDriverHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DriverInfos
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.IsDeleted != true, cancellationToken);

        if (entity == null) throw new ArgumentException($"Driver '{request.Id}' not found.");

        entity.DriverName = request.DriverName;
        entity.CNIC = request.CNIC;
        entity.MobileNo = request.MobileNo;
        entity.Email = request.Email;
        entity.Gender = request.Gender;
        entity.LicenceNo = request.LicenceNo;
        entity.DriverStatusId = request.DriverStatusId;
        entity.StatusDate = request.StatusDate;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver '{entity.DriverName}' updated successfully.");
    }
}

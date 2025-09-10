using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Drivers.Commands;
public record AddDriverCommand(
    string DriverName,
    string CNIC,
    string MobileNo,
    string? Email,
    string Gender,
    string LicenceNo,
    Guid DriverStatusId,
    DateTime StatusDate
) : IRequest<SuccessResponse<string>>;

// ADD
public class AddDriverHandler : IRequestHandler<AddDriverCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddDriverHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddDriverCommand request, CancellationToken cancellationToken)
    {
        var entity = new OLH_DriverInfo
        {
            DriverName = request.DriverName,
            CNIC = request.CNIC,
            MobileNo = request.MobileNo,
            Email = request.Email,
            Gender = request.Gender,
            LicenceNo = request.LicenceNo,
            DriverStatusId = request.DriverStatusId,
            StatusDate = request.StatusDate,
            IsActive = true,
            IsDeleted = false
        };

        _context.DriverInfos.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver '{entity.DriverName}' successfully added.");
    }
}

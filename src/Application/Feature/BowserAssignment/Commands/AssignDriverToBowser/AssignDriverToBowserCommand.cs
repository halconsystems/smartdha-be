using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.AssignDriverToBowser;
public record AssignDriverToBowserCommand(AssignDriverToBowserDto Dto) : IRequest<SuccessResponse<Guid>>;

public class AssignDriverToBowserCommandHandler : IRequestHandler<AssignDriverToBowserCommand, SuccessResponse<Guid>>
{
    private readonly IOLHApplicationDbContext _context;

    public AssignDriverToBowserCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AssignDriverToBowserCommand request, CancellationToken cancellationToken)
    {
        try { 
        var dto = request.Dto;
            if (dto.BowserRequestId == Guid.Empty)
                throw new ArgumentException("BowserRequestId cannot be null");

            if (dto.DriverId == Guid.Empty)
                throw new ArgumentException("DriverId cannot be null");

            if (dto.VehicleId == Guid.Empty)
                throw new ArgumentException("VehicleId cannot be null");

            // Verify Bowser Request exists
            var bowserRequest = await _context.BowserRequests
            .FirstOrDefaultAsync(br => br.Id == dto.BowserRequestId, cancellationToken);
        if (bowserRequest == null)
            throw new NotFoundException(nameof(OLH_BowserRequest), dto.BowserRequestId.ToString());

        // Verify Driver exists
        var driver = await _context.DriverInfos
            .FirstOrDefaultAsync(d => d.Id == dto.DriverId, cancellationToken);
        if (driver == null)
            throw new NotFoundException(nameof(OLH_DriverInfo), dto.DriverId.ToString());

        // Verify Vehicle exists
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == dto.VehicleId, cancellationToken);
        if (vehicle == null)
            throw new NotFoundException(nameof(OLH_Vehicle), dto.VehicleId.ToString());

            

            // Assign Driver & Vehicle & PlannedDeliveryDate
        bowserRequest.AssignedDriverId = dto.DriverId;
        bowserRequest.AssignedVehicleId = dto.VehicleId;
        bowserRequest.PlannedDeliveryDate = dto.PlannedDeliveryDate;

            // Get the "OnDuty" status from DriverStatuses table
            var onDutyStatus = await _context.DriverStatuses
                .FirstOrDefaultAsync(vs => vs.Status == Domain.Enums.DriverStatus.NotAvailable, cancellationToken);

            if (onDutyStatus == null)
                throw new Exception("Not Available status not found in VehicleStatuses");

            driver.DriverStatusId = onDutyStatus.Id;

           // _context.BowserRequests.Attach(bowserRequest);
           // _context.DriverInfos.Attach(driver);
            await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(
            bowserRequest.Id,
            "Driver assigned to bowser request successfully."
        );
        }
        catch (Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Error in AssignDriverToBowserCommandHandler:");

            Exception? inner = ex;
            while (inner != null)
            {
                sb.AppendLine(inner.Message);
                sb.AppendLine(inner.StackTrace);
                inner = inner.InnerException;
            }

            throw new Exception(sb.ToString());
        }
    }
}

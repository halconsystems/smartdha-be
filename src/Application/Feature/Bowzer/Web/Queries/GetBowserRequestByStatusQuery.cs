using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries;
public record GetBowserRequestByStatusQuery(BowserStatus? Status)
    : IRequest<SuccessResponse<List<BowserRequestDto>>>;

public class GetBowserRequestByStatusHandler : IRequestHandler<GetBowserRequestByStatusQuery, SuccessResponse<List<BowserRequestDto>>>
{
    private readonly IOLHApplicationDbContext _context;

    public GetBowserRequestByStatusHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<BowserRequestDto>>> Handle(GetBowserRequestByStatusQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BowserRequests
            .Include(b => b.Phase)
            .Include(b => b.BowserCapacity)
            .Include(b => b.AssignedDriver)
            .Include(b => b.AssignedVehicle)
            .Where(b => b.IsDeleted != true) // soft delete filter
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        var bowsers = await query.ToListAsync(cancellationToken);

        return new SuccessResponse<List<BowserRequestDto>>(bowsers.Select(MapToDto).ToList());
    }

    private static BowserRequestDto MapToDto(OLH_BowserRequest b) =>
        new BowserRequestDto
        {
            Id = b.Id,
            RequestNo = b.RequestNo,
            RequestDate = b.RequestDate,
            PhaseId = b.PhaseId,
            PhaseName = b.Phase?.Name, // adjust if property differs
            BowserCapacityId = b.BowserCapacityId,
            BowserCapacityName = b.BowserCapacity?.Capacity.ToString(),
            RequestedDeliveryDate = b.RequestedDeliveryDate,
            PlannedDeliveryDate = b.PlannedDeliveryDate,
            DeliveryDate = b.DeliveryDate,
            Ext = b.Ext,
            Street = b.Street,
            Address = b.Address,
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            AssignedDriverId = b.AssignedDriverId,
            AssignedDriverName = b.AssignedDriver?.DriverName,
            AssignedVehicleId = b.AssignedVehicleId,
            AssignedVehiclePlate = b.AssignedVehicle?.LicensePlateNumber,
            Amount = b.Amount,
            Currency = b.Currency,
            Status = b.Status,
            PaymentStatus = b.PaymentStatus,
            CustomerId = b.CustomerId,
            Notes = b.Notes
        };
}


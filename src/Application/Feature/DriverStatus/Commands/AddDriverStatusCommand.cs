using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.DriverStatus.Commands;
// Add
public record AddDriverStatusCommand(string Status) : IRequest<SuccessResponse<string>>;

// ADD
public class AddDriverStatusHandler : IRequestHandler<AddDriverStatusCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddDriverStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddDriverStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = new OLH_DriverStatus
        {
            Status = request.Status,
            IsActive = true,
            IsDeleted = false
        };

        _context.DriverStatuses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Driver status '{entity.Status}' added successfully.");
    }
}

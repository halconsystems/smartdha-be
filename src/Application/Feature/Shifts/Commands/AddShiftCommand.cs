using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Shifts.Commands;
public record AddShiftCommand(string ShiftName, TimeOnly StartTime, TimeOnly EndTime)
    : IRequest<SuccessResponse<string>>;
public class AddShiftHandler : IRequestHandler<AddShiftCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddShiftHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = new OLH_Shift
        {
            ShiftName = request.ShiftName,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsActive = true,
            IsDeleted = false
        };

        _context.Shifts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Shift '{entity.ShiftName}' added successfully.");
    }
}

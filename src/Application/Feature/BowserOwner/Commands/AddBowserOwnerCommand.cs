using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserOwner.Commands;

public record AddBowserOwnerCommand(string OwnerName) : IRequest<SuccessResponse<string>>;

public class AddBowserOwnerHandler : IRequestHandler<AddBowserOwnerCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddBowserOwnerHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddBowserOwnerCommand request, CancellationToken cancellationToken)
    {
        var entity = new OLH_VehicleOwner
        {
            OwnerName = request.OwnerName,
           
        };

        _context.VehicleOwners.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Owner '{entity.OwnerName}' added successfully.");
    }
}

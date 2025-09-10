using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
public record AddBowserMakeCommand(string MakeName) : IRequest<SuccessResponse<string>>;

public class AddBowserMakeHandler : IRequestHandler<AddBowserMakeCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddBowserMakeHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddBowserMakeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.VehicleMakes
            .AnyAsync(x => x.MakeName == request.MakeName && x.IsDeleted!=true, cancellationToken);

        if (exists)
            throw new ArgumentException($"Bowser Make '{request.MakeName}' already exists.");

        var entity = new OLH_VehicleMake { MakeName = request.MakeName };
        _context.VehicleMakes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Make '{request.MakeName}' successfully added.");
    }
}

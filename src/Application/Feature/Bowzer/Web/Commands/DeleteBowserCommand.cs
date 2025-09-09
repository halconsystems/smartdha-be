using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Commands;
public record DeleteBowserCommand(Guid Id) : IRequest<SuccessResponse<string>>;

public class DeleteBowserCommandHandler : IRequestHandler<DeleteBowserCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;

    public DeleteBowserCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteBowserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Vehicles.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new ArgumentException($"Vehicle with Id '{request.Id}' not found.");
        }

        _context.Vehicles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle '{entity.LicensePlateNumber}' successfully deleted.");
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.DeleteLaggagePass;

public class DeleteLaggagePassCommandHandler
    : IRequestHandler<DeleteLaggagePassCommand, DeleteLaggagePassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public DeleteLaggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<DeleteLaggagePassResponse> Handle(DeleteLaggagePassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.LaggagePasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Laggage Pass Not Found");

        _smartdhaDbContext.LaggagePasses.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteLaggagePassResponse
        {
            Message = "Laggage Pass Deleted Successfully"
        };
    }
}

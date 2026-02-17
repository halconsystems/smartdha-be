using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.DeleteLuggagePass;

public class DeleteLuggagePassCommandHandler
    : IRequestHandler<DeleteLuggagePassCommand, DeleteLuggagePassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public DeleteLuggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<DeleteLuggagePassResponse> Handle(DeleteLuggagePassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.LuggagePasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Luggage Pass Not Found");

        _smartdhaDbContext.LuggagePasses.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteLuggagePassResponse
        {
            Message = "Luggage Pass Deleted Successfully"
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdatePanicResponder;
public record UpdatePanicResponderCommand(
        Guid Id,
        string Name,
        string CNIC,
        string PhoneNumber,
        string Email,
        string Gender,
        Guid EmergencyTypeId
    ) : IRequest<Unit>;

public class UpdatePanicResponderCommandHandler : IRequestHandler<UpdatePanicResponderCommand, Unit>
{
    private readonly IApplicationDbContext _ctx;

    public UpdatePanicResponderCommandHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Unit> Handle(UpdatePanicResponderCommand request, CancellationToken cancellationToken)
    {
        var responder = await _ctx.PanicResponders
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Panic Responder not found.");

        // Optional: Validate emergency type exists
        var emergencyTypeExists = await _ctx.EmergencyTypes
            .AnyAsync(x => x.Id == request.EmergencyTypeId, cancellationToken);

        if (!emergencyTypeExists)
            throw new KeyNotFoundException("Invalid Emergency Type.");

        responder.Name = request.Name;
        responder.CNIC = request.CNIC;
        responder.PhoneNumber = request.PhoneNumber;
        responder.Email = request.Email;
        responder.Gender = request.Gender;
        responder.EmergencyTypeId = request.EmergencyTypeId;
        responder.LastModified = DateTime.Now;

        await _ctx.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

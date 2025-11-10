using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.AddPanicResponder;
public record AddPanicResponderCommand(
        string Name,
        string CNIC,
        string PhoneNumber,
        string Email,
        string Gender,
        Guid EmergencyTypeId
    ) : IRequest<Guid>;

public class AddPanicResponderCommandHandler : IRequestHandler<AddPanicResponderCommand, Guid>
{
    private readonly IApplicationDbContext _ctx;

    public AddPanicResponderCommandHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Guid> Handle(AddPanicResponderCommand request, CancellationToken cancellationToken)
    {
        // Validate EmergencyType exists
        var emergencyType = await _ctx.EmergencyTypes
            .FirstOrDefaultAsync(x => x.Id == request.EmergencyTypeId, cancellationToken)
            ?? throw new KeyNotFoundException("Invalid Emergency Type.");

        var entity = new PanicResponder
        {
            Name = request.Name,
            CNIC = request.CNIC,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Gender = request.Gender,
            EmergencyTypeId = request.EmergencyTypeId,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.Now
        };

        _ctx.PanicResponders.Add(entity);
        await _ctx.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;

public class UpdateNonMemberVerificationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public VerificationStatus Status { get; set; }
    public bool IsActive { get; set; }

}

public class UpdateNonMemberVerificationCommandHandler : IRequestHandler<UpdateNonMemberVerificationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateNonMemberVerificationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateNonMemberVerificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NonMemberVerifications
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            // Handle not found case (you can throw custom NotFoundException if needed)
            return false;
        }

        entity.Status = request.Status;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}


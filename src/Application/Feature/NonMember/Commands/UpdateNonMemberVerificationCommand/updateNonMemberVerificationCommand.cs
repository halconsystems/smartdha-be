using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.NonMember.Commands.UpdateNonMemberVerificationCommand;

public class UpdateNonMemberVerificationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public VerificationStatus Status { get; set; }
    public bool IsActive { get; set; }

    public string remarks { get; set; } = default!;

}

public class UpdateNonMemberVerificationCommandHandler : IRequestHandler<UpdateNonMemberVerificationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateNonMemberVerificationCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
        var entity_User = await _userManager.Users
     .FirstOrDefaultAsync(x => x.Id == entity.UserId.ToString(), cancellationToken);
        if (entity_User == null)
        {
            return false;
        }
        entity.Status = request.Status;
        entity.Remarks = request.remarks;
        entity.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        entity_User.IsVerified = request.IsActive;
        await _userManager.UpdateAsync(entity_User);

        return true;
    }
}


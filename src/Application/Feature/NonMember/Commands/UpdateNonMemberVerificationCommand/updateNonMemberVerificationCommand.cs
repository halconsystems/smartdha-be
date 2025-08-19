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
    public bool IsOtpRequired { get; set; } = false;
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
            return false;

        var entity_User = await _userManager.FindByIdAsync(entity.UserId.ToString());
        if (entity_User == null)
            return false;

        entity.Status = request.Status;
        entity.Remarks = request.remarks;
        entity.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        entity_User.IsVerified = request.IsActive;
        entity_User.IsOtpRequired=request.IsOtpRequired;
        var result = await _userManager.UpdateAsync(entity_User);

        return result.Succeeded;
    }

}


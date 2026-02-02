using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Command;

public class UpdateMemberShipRequestCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public VerificationStatus Status { get; set; }

}

public class UpdateMemberShipRequestCommandHandler : IRequestHandler<UpdateMemberShipRequestCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateMemberShipRequestCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> Handle(UpdateMemberShipRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MemberRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return false;


        entity.Status = request.Status;
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {

            throw;
        }


        return true;
    }

}


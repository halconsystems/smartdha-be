using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard.Commands.AddMemberTypeModuleAssignments;
public class AddMemberTypeModuleAssignmentsCommand : IRequest<SuccessResponse<string>>
{
    // Default value set to UserType.Member
    public UserType UserType { get; set; } = UserType.Member;

    public Guid ModuleId { get; set; } = new();
}
public class AddMemberTypeModuleAssignmentCommandHandler
    : IRequestHandler<AddMemberTypeModuleAssignmentsCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public AddMemberTypeModuleAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddMemberTypeModuleAssignmentsCommand request, CancellationToken cancellationToken)
    {
        // Check if this module is already assigned to the given UserType
        bool exists = await _context.MemberTypeModuleAssignments
            .AnyAsync(x => x.UserType == request.UserType && x.ModuleId == request.ModuleId, cancellationToken);

        if (exists)
        {
            return SuccessResponse<string>.FromMessage($"Module is already assigned to {request.UserType}.");
        }

        // Insert new assignment
        var assignment = new MemberTypeModuleAssignment
        {
            UserType = request.UserType,
            ModuleId = request.ModuleId
        };

        await _context.MemberTypeModuleAssignments.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return SuccessResponse<string>.FromMessage($"Module assigned successfully to {request.UserType}.");
    }
}


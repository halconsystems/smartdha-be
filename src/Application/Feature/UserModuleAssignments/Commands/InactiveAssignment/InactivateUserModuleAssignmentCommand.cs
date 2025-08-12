using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.InactiveAssignment;
public class InactivateUserModuleAssignmentCommand : IRequest<SuccessResponse<List<Guid>>>
{
    [Required]
    public List<Guid> Ids { get; set; } = new();
}

public class InactivateUserModuleAssignmentCommandHandler : IRequestHandler<InactivateUserModuleAssignmentCommand, SuccessResponse<List<Guid>>>
{
    private readonly IApplicationDbContext _ctx;

    public InactivateUserModuleAssignmentCommandHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<Guid>>> Handle(InactivateUserModuleAssignmentCommand request, CancellationToken ct)
    {
        var updatedIds = new List<Guid>();

        var assignments = await _ctx.UserModuleAssignments
            .Where(x => request.Ids.Contains(x.Id) && x.IsActive == true)
            .ToListAsync(ct);

        if (assignments.Count == 0)
            throw new KeyNotFoundException("No active assignments found to inactivate.");

        foreach (var assignment in assignments)
        {
            assignment.IsActive = false;
            assignment.LastModified = DateTime.UtcNow;

            updatedIds.Add(assignment.Id);
        }

        await _ctx.SaveChangesAsync(ct);
        return Success.Update(updatedIds, "Assignments inactivated successfully.");
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.DeleteAssignment;
public class DeleteUserModuleAssignmentCommand : IRequest<SuccessResponse<List<Guid>>>
{
    [Required]
    public List<Guid> Ids { get; set; } = new();

    public bool HardDelete { get; set; } = false; // default to soft delete
}
public class DeleteUserModuleAssignmentCommandHandler : IRequestHandler<DeleteUserModuleAssignmentCommand, SuccessResponse<List<Guid>>>
{
    private readonly IApplicationDbContext _ctx;

    public DeleteUserModuleAssignmentCommandHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<Guid>>> Handle(DeleteUserModuleAssignmentCommand request, CancellationToken ct)
    {
        var affectedIds = new List<Guid>();

        var assignments = await _ctx.UserModuleAssignments
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(ct);

        if (assignments.Count == 0)
            throw new KeyNotFoundException("No matching assignments found.");

        foreach (var assignment in assignments)
        {
            if (request.HardDelete)
            {
                _ctx.UserModuleAssignments.Remove(assignment);
            }
            else
            {
                assignment.IsDeleted = true;
                assignment.IsActive = false;
                assignment.LastModified = DateTime.UtcNow;
            }

            affectedIds.Add(assignment.Id);
        }

        await _ctx.SaveChangesAsync(ct);
        return Success.Delete(affectedIds, request.HardDelete ? "Hard deleted successfully." : "Soft deleted successfully.");
    }
}

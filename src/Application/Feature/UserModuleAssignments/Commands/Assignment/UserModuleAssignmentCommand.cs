using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Commands.Assignment;
public class CreateUserModuleAssignmentCommand : IRequest<SuccessResponse<List<Guid>>>
{
    [Required]
    public string UserId { get; set; } = default!;
    public List<Guid> ModuleIds { get; set; }= new();
}
public class CreateUserModuleAssignmentCommandHandler : IRequestHandler<CreateUserModuleAssignmentCommand, SuccessResponse<List<Guid>>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserModuleAssignmentCommandHandler(IApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<List<Guid>>> Handle(CreateUserModuleAssignmentCommand request, CancellationToken ct)
    {
        // Validate user
        var userExists = await _userManager.Users.AnyAsync(u => u.Id == request.UserId, ct);
        if (!userExists)
            throw new KeyNotFoundException("User not found.");

        // Ensure distinct module IDs
        var distinctModuleIds = request.ModuleIds.Distinct().ToList() ?? new List<Guid>();

        // Validate module IDs
        var validModuleIds = await _ctx.Modules
            .Where(m => distinctModuleIds.Contains(m.Id) && (m.IsDeleted == false || m.IsDeleted == null))
            .Select(m => m.Id)
            .ToListAsync(ct);

        if (validModuleIds.Count != distinctModuleIds.Count)
            throw new ArgumentException("One or more provided ModuleIds are invalid.");

        // Fetch existing user-module assignments
        var existingAssignments = await _ctx.UserModuleAssignments
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(ct);

        var existingModuleIds = existingAssignments.Select(x => x.ModuleId).ToList();

        // Determine which to add and which to remove
        var toAdd = distinctModuleIds.Except(existingModuleIds).ToList();
        var toRemove = existingModuleIds.Except(distinctModuleIds).ToList();

        var hasChanges = false;
        var createdIds = new List<Guid>();

        // Remove unselected assignments
        if (toRemove.Any())
        {
            var assignmentsToRemove = existingAssignments
                .Where(x => toRemove.Contains(x.ModuleId))
                .ToList();

            _ctx.UserModuleAssignments.RemoveRange(assignmentsToRemove);
            hasChanges = true;
        }

        // Add new assignments
        foreach (var moduleId in toAdd)
        {
            var assignment = new UserModuleAssignment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ModuleId = moduleId,
                Created = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            await _ctx.UserModuleAssignments.AddAsync(assignment, ct);
            createdIds.Add(assignment.Id);
            hasChanges = true;
        }

        if (hasChanges)
            await _ctx.SaveChangesAsync(ct);

        return Success.Created(createdIds, hasChanges 
            ? "User module assignments updated successfully."  : "No changes were made to user module assignments.");
    }
}




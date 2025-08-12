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

        // Validate module IDs
        var existingModuleIds = await _ctx.Modules
            .Where(m => request.ModuleIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync(ct);

        var invalidModuleIds = request.ModuleIds.Except(existingModuleIds).ToList();
        if (invalidModuleIds.Any())
            throw new KeyNotFoundException($"The following module IDs are invalid: {string.Join(", ", invalidModuleIds)}");

        var createdIds = new List<Guid>();

        foreach (var moduleId in request.ModuleIds)
        {
            // Skip duplicates
            var alreadyExists = await _ctx.UserModuleAssignments
                .AnyAsync(x => x.UserId == request.UserId && x.ModuleId == moduleId, ct);

            if (alreadyExists) continue;

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
        }

        if (createdIds.Count > 0)
            await _ctx.SaveChangesAsync(ct);

        return Success.Created(createdIds, "Modules assigned successfully.");
    }
}




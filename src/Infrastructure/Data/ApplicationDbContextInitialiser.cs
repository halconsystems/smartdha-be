using System.Runtime.InteropServices;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DHAFacilitationAPIs.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }


    public async Task TrySeedAsync()
    {
        // 1️⃣ Seed Roles
        foreach (var role in AllRoles.GetAllRoles())
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create role {Role}: {Errors}",
                        role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        var purposes = new List<MembershipPurpose>
        {
            new() { Title = "Visiting Family" },
            new() { Title = "Business" },
            new() { Title = "Property Inquiry" },
            new() { Title = "Events / Club Access" }
        };

        if (!_context.MembershipPurposes.Any())
        {
            _context.MembershipPurposes.AddRange(purposes);
            await _context.SaveChangesAsync();
        }

        var assignments = new List<RoleAssignment>
        {
            new() { ParentRole = "SuperAdministrator", ChildRole = "Administrator" },
            new() { ParentRole = "SuperAdministrator", ChildRole = "Admin" },
            new() { ParentRole = "SuperAdministrator", ChildRole = "User" },

            new() { ParentRole = "Administrator", ChildRole = "Admin" },
            new() { ParentRole = "Administrator", ChildRole = "User" },

            new() { ParentRole = "Admin", ChildRole = "User" }
        };

        if (!_context.RoleAssignments.Any())
        {
            _context.RoleAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();
        }

        // 2️⃣ Seed Users

        async Task<ApplicationUser> SeedUser(string name, string email, string role, string cnic)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null) return existing;

            var user = new ApplicationUser
            {
                Name = name,
                Email = email,
                UserName = email,
                CNIC = cnic,
                EmailConfirmed = true,
                AppType = AppType.Web,
                UserType = UserType.Employee
            };

            var result = await _userManager.CreateAsync(user, $"{role}123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return user;
            }

            _logger.LogError("Failed to create {Role} user: {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
            return null!;
        }

        var superAdmin = await SeedUser("Super Admin", "superadministrator@dhakarachi.org", AllRoles.SuperAdministrator, "1234567890123");
        var administrator = await SeedUser("Administrator", "administrator@dhakarachi.org", AllRoles.Administrator, "1234567890124");
        var admin = await SeedUser("Admin", "admin@dhakarachi.org", AllRoles.Admin, "1234567890125");

        // 3️⃣ Seed Modules + SubModules

        if (!_context.Modules.Any())
        {
            var webModules = new List<Module>
        {
            new() { Name = "Non Member Approval", Title ="Non Member Approval", Description="Non Member Approval",Remarks="Non Member Approval", AppType = AppType.Web, SubModules = new List<SubModule> {
                new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "User Management", Title ="User Management", Description="User Management",Remarks="User Management", AppType = AppType.Web, SubModules = new List<SubModule> {
                new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "Announcement", Title ="Announcement", Description="Announcement",Remarks="Announcement", AppType = AppType.Web, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard" }
            }},
            new() { Name = "Club Management", Title ="Club Management", Description="Club Management",Remarks="Club Management", AppType = AppType.Web, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
        };

            var mobileModules = new List<Module>
        {
            new() { Name = "Club Management", Title ="Club Management", Description="Club Management",Remarks="Club Management", AppType = AppType.Mobile, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "Bowser", Title = "Bowser", Description="Bowser",Remarks="Bowser", AppType = AppType.Mobile, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "Property", Title = "Property", Description="Property",Remarks="Property", AppType = AppType.Mobile, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "Panic Button", Title = "Panic Button", Description="Panic Button", Remarks="Panic Button", AppType = AppType.Mobile, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "GIS",Title = "GIS", Description="GIS", Remarks="GIS", AppType = AppType.Mobile, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
            new() { Name = "QR Code",Title = "QR Code", Description="QR Code", Remarks="QR Code", AppType = AppType.Mobile, SubModules = new List<SubModule> {
               new() { Name = "Dashboard",Description="Dashboard"  }
            }},
        };

            await _context.Modules.AddRangeAsync(webModules.Concat(mobileModules));
            await _context.SaveChangesAsync();
        }

        // 4️⃣ Assign ALL modules to SuperAdmin

        var allModules = await _context.Modules.ToListAsync();
        var superAdminAssignments = allModules.Select(m => new UserModuleAssignment
        {
            UserId = superAdmin.Id,
            ModuleId = m.Id,
            Created = DateTime.UtcNow,
            CreatedBy = superAdmin.Id
        });

        var assignedModuleIds = _context.UserModuleAssignments
            .Where(x => x.UserId.ToString() == superAdmin.Id)
            .Select(x => x.ModuleId)
            .ToHashSet();

        var newAssignments = superAdminAssignments
            .Where(x => !assignedModuleIds.Contains(x.ModuleId));

        _context.UserModuleAssignments.AddRange(newAssignments);

        // 5️⃣ Assign FULL PERMISSIONS to SuperAdmin for all SubModules

        if (!_context.RolePermissions.Any(x => x.RoleName == AllRoles.SuperAdministrator))
        {
            var allSubModules = await _context.SubModules.ToListAsync();

            var permissions = allSubModules.Select(sm => new RolePermission
            {
                RoleName = AllRoles.SuperAdministrator,
                SubModuleId = sm.Id,
                CanRead = true,
                CanWrite = true,
                CanDelete = true
            });

            await _context.RolePermissions.AddRangeAsync(permissions);
        }

        await _context.SaveChangesAsync();
    }

}

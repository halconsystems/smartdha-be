using System.Runtime.InteropServices;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
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

        await initialiser.SeedMerchantsAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PaymentDbContext _db;
    private readonly ISecureKeyProtector _secureKeyProtector;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, PaymentDbContext db,ISecureKeyProtector secureKeyProtector)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _secureKeyProtector = secureKeyProtector;
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
        // 1️⃣ Super Admin User
        var superAdminEmail = "superadministrator@dhakarachi.org";
        var superAdminPassword = "SuperAdministrator1!";

        var existingUser = await _userManager.FindByEmailAsync(superAdminEmail);
        ApplicationUser superAdmin;

        if (existingUser == null)
        {
            superAdmin = new ApplicationUser
            {
                Name = "Super Administrator",
                Email = superAdminEmail,
                UserName = superAdminEmail,
                CNIC = "1234567890123",
                EmailConfirmed = true,
                AppType = AppType.Web,
                UserType = UserType.Employee,
                MobileNo= "03000000000",
                MEMPK="2025",
            };

            var result = await _userManager.CreateAsync(superAdmin, superAdminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create SuperAdmin: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            superAdmin = existingUser;
        }

//        // 2️⃣ Seed Modules
//        if (!_context.Modules.Any())
//        {
//            // 📱 Mobile Modules (no submodules)
//            var mobileModules = new List<Module>
//        {
//          new() {
//        Value = "PanicButton",
//        DisplayName = "Panic Button",
//        Name = "PanicButton",
//        Description = "Mobile panic button alerts and quick actions",
//        Title = "Panic Button",
//        Remarks = "Critical safety feature",
//        AppType = AppType.Mobile,
//        URL = "app://panic-button"
//    },
//    new() {
//        Value = "PropertyManagement",
//        DisplayName = "Property Management",
//        Name = "PropertyManagement",
//        Description = "Manage property details from mobile",
//        Title = "Property Management",
//        Remarks = "Mobile access",
//        AppType = AppType.Mobile,
//        URL = "app://property-management"
//    },
//    new() {
//        Value = "ClubManagement",
//        DisplayName = "Club Management",
//        Name = "ClubManagement",
//        Description = "Mobile access to club reservations and payments",
//        Title = "Club Management",
//        Remarks = "For members",
//        AppType = AppType.Mobile,
//        URL = "app://club-management"
//    },
//    new() {
//        Value = "Bowzer",
//        DisplayName = "Bowzer",
//        Name = "Bowzer",
//        Description = "Mobile Bowzer services",
//        Title = "Bowzer",
//        Remarks = "Bowzer on mobile",
//        AppType = AppType.Mobile,
//        URL = "app://bowzer"
//    },
//    new() {
//        Value = "QRCode",
//        DisplayName = "QR Code",
//        Name = "QRCode",
//        Description = "Scan and generate QR Codes",
//        Title = "QR Code",
//        Remarks = "Mobile QR services",
//        AppType = AppType.Mobile,
//        URL = "app://qr-code"
//    }
//};


//            // 💻 Web Modules (with submodules)
//            var webModules = new List<Module>
//{
//    new() {
//        Value = "PanicButton",
//        DisplayName = "Panic Button",
//        Name = "PanicButton",
//        Description = "Manage panic button alerts and monitoring",
//        Title = "Panic Button Module",
//        Remarks = "Critical feature for emergency response",
//        AppType = AppType.Web,
//        URL = "/panic-button",
//        SubModules = new List<SubModule> {
//            new() {
//                Value = "WebPanic.Dashboard",
//                DisplayName = "Dashboard",
//                Name = "WebPanicDashboard",
//                Description = "Overview of panic button alerts",
//                RequiresPermission = false
//            }
//        }
//    },
//    new() {
//        Value = "ClubManagement",
//        DisplayName = "Club Management",
//        Name = "ClubManagement",
//        Description = "Manage club activities, reservations, and payments",
//        Title = "Club Management Module",
//        Remarks = "Important for member services",
//        AppType = AppType.Web,
//        URL = "/club-management",
//        SubModules = new List<SubModule> {
//            new() {
//                Value = "WebClub.Dashboard",
//                DisplayName = "Dashboard",
//                Name = "WebClubDashboard",
//                Description = "Overview of club activities",
//                RequiresPermission = false
//            },
//            new() {
//                Value = "WebClub.RoomReservation",
//                DisplayName = "Room Reservation",
//                Name = "WebClubRoomReservation",
//                Description = "Manage club room reservations",
//                RequiresPermission = true
//            },
//            new() {
//                Value = "WebClub.Payment",
//                DisplayName = "Payment Received",
//                Name = "WebClubPayment",
//                Description = "Track and approve club payments",
//                RequiresPermission = true
//            }
//        }
//    },
//    new() {
//        Value = "NonMemberApproval",
//        DisplayName = "Non Member Approval",
//        Name = "NonMemberApproval",
//        Description = "Handle non-member verification and approval requests",
//        Title = "Non Member Approval Module",
//        Remarks = "Workflow module",
//        AppType = AppType.Web,
//        URL = "/non-member-approval",
//        SubModules = new List<SubModule> {
//            new() {
//                Value = "NMA.Dashboard",
//                DisplayName = "Dashboard",
//                Name = "NMADashboard",
//                Description = "Overview of non-member approval status",
//                RequiresPermission = false
//            },
//            new() {
//                Value = "NMA.Request",
//                DisplayName = "Request",
//                Name = "NMARequest",
//                Description = "Approve or reject non-member requests",
//                RequiresPermission = true
//            }
//        }
//    }
//};


//            await _context.Modules.AddRangeAsync(mobileModules.Concat(webModules));
//            await _context.SaveChangesAsync();
//        }

        // 3️⃣ Seed SuperAdmin Role
        //var superAdminRole = await _context.AppRoles
        //    .FirstOrDefaultAsync(r => r.Name == "SuperAdministrator");

        //if (superAdminRole == null)
        //{
        //    superAdminRole = new AppRole
        //    {
        //        Name = "SuperAdministrator",
        //        IsSystemRole = true
        //    };
        //    _context.AppRoles.Add(superAdminRole);
        //    await _context.SaveChangesAsync();
        //}

        // 4️⃣ Assign Role to SuperAdmin User
        //var roleAssigned = await _context.AppUserRoles
        //    .AnyAsync(ur => ur.UserId == superAdmin.Id && ur.RoleId == superAdminRole.Id);

        //if (!roleAssigned)
        //{
        //    var userRole = new AppUserRole
        //    {
        //        UserId = superAdmin.Id,
        //        RoleId = superAdminRole.Id
        //    };
        //    _context.AppUserRoles.Add(userRole);
        //    await _context.SaveChangesAsync();
        //}

        // 5️⃣ Assign Full Permissions on All SubModules
        //var allSubModules = await _context.SubModules.ToListAsync();

        //foreach (var sm in allSubModules)
        //{
        //    bool exists = await _context.AppRolePermissions
        //        .AnyAsync(rp => rp.RoleId == superAdminRole.Id && rp.SubModuleId == sm.Id);

        //    if (!exists)
        //    {
        //        _context.AppRolePermissions.Add(new AppRolePermission
        //        {
        //            RoleId = superAdminRole.Id,
        //            SubModuleId = sm.Id,
        //            AllowedActions = "Read,Write,Delete,Approve,Reject"
        //        });
        //    }
        //}


        await _context.SaveChangesAsync();
    }

    public async Task SeedMerchantsAsync()
    {
        foreach (var (code, name, merchantId) in PayMerchantSeedData.Merchants)
        {
            var exists = await _db.PayMerchants
                .AnyAsync(x => x.Code == code);

            if (exists)
                continue;

            var secureKeyPlain = $"wZLV_F2VcDmJ9voSHp6t8ND";
            // Replace above with actual secure key before PROD

            var merchant = new PayMerchant
            {
                Id = Guid.NewGuid(),
                Code = code,
                DisplayName = name,
                MerchantId = "244018",
                SmartPayMerchantId= merchantId,
                SecureKeyEncrypted = _secureKeyProtector.Encrypt(secureKeyPlain),
                Gateway = "PAYFAST",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.PayMerchants.Add(merchant);
        }

        await _db.SaveChangesAsync();
    }

    public static class PayMerchantSeedData
    {
        public static IReadOnlyList<(string Code, string Name, string MerchantId)> Merchants
            => new List<(string, string, string)>
        {
        ("DHA_SECURITY", "DHA Security & Vigilance Directorate", "6005"),
        ("DHA_RENTAL", "DHA Karachi (Rental Billing)", "6010"),

        ("MARINA_CLUB", "Marina Club", "6031"),
        ("ZAMZAMA_CLUB", "DHA Zamzama Club", "6032"),
        ("BEACHVIEW_CLUB", "DHA Beachview Club", "6033"),
        ("AUTHORITY_CLUB", "DHA Authority Club", "6034"),
        ("SUNSET_CLUB", "DHA Sunset Club", "6035"),
        ("CREEK_CLUB", "DHA Creek Club", "6036"),
        ("MOIN_KHAN_ACADEMY", "Moin Khan Academy", "6037"),
        ("COUNTRY_GOLF_CLUB", "DHA Country & Golf Club", "6038"),

        ("DHACSS_WOMEN_COLLEGE", "DHACSS College For Women", "6111"),
        ("DHACSS_BOYS_COLLEGE", "DHACSS Degree College (Boys)", "6112"),
        ("DHACSS_CREEK", "DHACSS Creek Campus", "6113"),
        ("DHACSS_SKBZ", "DHACSS SKBZ Campus", "6114"),
        ("DHACSS_SEAVIEW", "DHACSS Seaview Campus", "6115"),
        ("DHACSS_PHASE4", "DHACSS Phase-IV Campus", "6116"),
        ("DHACSS_PHASE7", "DHACSS PH-VII Campus", "6117"),
        ("DHACSS_BEACHVIEW", "DHACSS Beachview Campus", "6118"),
        ("DHACSS_MONTESSORI", "DHACSS Montessori", "6119"),
        ("DHA_KIDS", "DHA Kids Campus", "6120"),
        ("DHACSS_TOOBA", "DHACSS Tooba Campus", "6121"),
        ("DHACSS_NEELAM", "DHACSS Neelam Campus", "6122"),
        ("DHACSS_IGRA", "DHACSS Igra Campus", "6123"),
        ("PMS", "Property Management System", "0000")
        };
    }

}

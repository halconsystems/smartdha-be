using System.Text;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Infrastructure.Data;
using DHAFacilitationAPIs.Infrastructure.Data.Interceptors;
using DHAFacilitationAPIs.Infrastructure.Identity;
using DHAFacilitationAPIs.Infrastructure.Notifications;
using DHAFacilitationAPIs.Infrastructure.Service;
using DHAFacilitationAPIs.Infrastructure.Service.Geocoding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace DHAFacilitationAPIs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        // services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(connectionString);
            //options.UseMySql(
            //              ServerVersion.AutoDetect(connectionString),
            //              options => options.EnableRetryOnFailure(
            //        maxRetryCount: 5,
            //        maxRetryDelay: System.TimeSpan.FromSeconds(30),
            //        errorNumbersToAdd: null));
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        //OLMRS Database

        var olmrConnection = configuration.GetConnectionString("OLMRSConnection");
        Guard.Against.Null(olmrConnection, message: "Connection string 'OlmrConnection' not found.");

        services.AddDbContext<OLMRSApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(olmrConnection);
        });

        services.AddScoped<IOLMRSApplicationDbContext>(provider =>
            provider.GetRequiredService<OLMRSApplicationDbContext>());

        //OLH Database
        var olhCon = configuration.GetConnectionString("OLHConnection");
        Guard.Against.Null(olhCon, message: "Connection string 'OLHConnection' not found.");
        services.AddDbContext<OLHApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(olhCon);

        });
        services.AddScoped<IOLHApplicationDbContext>(provider =>
        provider.GetRequiredService<OLHApplicationDbContext>());

        //PMS

        var pms = configuration.GetConnectionString("PMSConnection");
        Guard.Against.Null(pms, message: "Connection string 'PMSConnection' not found.");
        services.AddDbContext<PMSApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(pms);

        });
        services.AddScoped<IPMSApplicationDbContext>(provider =>
        provider.GetRequiredService<PMSApplicationDbContext>());


        //LMS

        var lms = configuration.GetConnectionString("LMSConnection");
        Guard.Against.Null(lms, message: "Connection string 'LMSConnection' not found.");
        services.AddDbContext<LaundrySystemDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(lms);

        });
        services.AddScoped<ILaundrySystemDbContext>(provider =>
        provider.GetRequiredService<LaundrySystemDbContext>());


        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<IProcedureService, StoredProcedures>();


        services.AddIdentityCore<ApplicationUser>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireDigit = true;
            opt.Password.RequireUppercase = true;

        }).AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();



        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie("Identity.Application", options =>
        { // all your options
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            options.Cookie.SameSite = SameSiteMode.Lax;
        }).AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? string.Empty))
                    };

                    o.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context =>
                        {
                            context.Response.OnStarting(async () =>
                            {
                                context.HandleResponse();
                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "text/plain";
                                await context.Response.WriteAsync("401 Not authorized");
                            });

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.OnStarting(async () =>
                            {
                                context.Response.StatusCode = 403;
                                context.Response.ContentType = "text/plain";
                                await context.Response.WriteAsync("403 forbidden");
                            });

                            return Task.CompletedTask;
                        },
                    };
                });

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthorization();

        services
            .Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));

        services.Configure<GoogleMapsOptions>(
            configuration.GetSection(GoogleMapsOptions.SectionName));

        // HttpClient-based Google Geocoding Service
        services.AddHttpClient<IGeocodingService, GoogleGeocodingService>();

        services.AddHttpClient<INotificationService, FirebaseNotificationService>();

        services.AddScoped<IFirebaseTokenProvider, FirebaseTokenProvider>();

        services.AddSingleton<IVehicleLocationStore, VehicleLocationStore>();


        return services;
    }
}

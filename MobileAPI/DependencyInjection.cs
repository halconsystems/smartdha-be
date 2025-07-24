using System.Text.Json;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using MobileAPI.Infrastructure;
using MobileAPI.Services;

namespace MobileAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddMobileAPIServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        //   services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddSwagger();
        services.AddCustomCors();

        services.AddScoped<IUser, CurrentUser>();
        services.AddHttpContextAccessor();
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddMvc();
        services.AddControllers(options =>
        {
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
            config.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        services.AddRouting(options => options.LowercaseUrls = true);

        return services;
    }
}

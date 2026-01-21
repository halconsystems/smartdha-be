using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Infrastructure.Data;
using DHAFacilitationAPIs.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MobileAPI;
using MobileAPI.Hubs;
using MobileAPI.Infrastructure;
using MobileAPI.Middlewares;
using MobileAPI.Services;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;


var options = new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "wwwroot" //ensure this matches your actual web root folder
};

var builder = WebApplication.CreateBuilder(options);

var logDirectory = Path.Combine(
    builder.Environment.WebRootPath ?? "wwwroot",
    "logs"
);

Directory.CreateDirectory(logDirectory);

var logPath = Path.Combine(logDirectory, "api-log-.txt");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
    .MinimumLevel.Override("StackExchange.Redis", LogEventLevel.Error)

    //API & CQRS logs
    .WriteTo.File(
        "wwwroot/logs/api-log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information
    )

    //Infra errors only
    .WriteTo.File(
        "wwwroot/logs/infra-error-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Error
    )
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMobileAPIServices(builder.Configuration);

builder.Services.AddSingleton<DapperConnectionFactory>();
builder.Services.AddHttpClient<ISmsService, SmsService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();

builder.Services.Configure<SmartPayOptions>(builder.Configuration.GetSection("SmartPay"));
builder.Services.AddScoped<ISmartPayService, SmartPayService>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "RBAC_";
});
builder.Services.AddMemoryCache(); // backup fallback
builder.Services.AddScoped<IPermissionCache, RedisPermissionCache>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SetPasswordPolicy", policy =>
    {
        policy.RequireClaim("purpose", "set_password");
    });
    options.AddPolicy("SetOTPPolicy", policy =>
    {
        policy.RequireClaim("purpose", "verify_otp");
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// (D) SignalR + Redis backplane (read from appsettings)
var redisConn = builder.Configuration.GetConnectionString("Redis");
builder.Services
    .AddSignalR()
    .AddStackExchangeRedis(redisConn!, opts =>
    {
        // Explicit channel prefix to avoid obsolete API and to keep both hosts on same bus
        opts.Configuration.ChannelPrefix = RedisChannel.Literal("panic");
    });

builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);

// (F) Host-specific realtime adapter (Mobile)
builder.Services.AddScoped<IPanicRealtime, PanicRealtimeMobileAdapter>();
builder.Services.AddScoped<IOrderRealTime, OrderRealtimeMobileAdapter>();
builder.Services.AddScoped<ICaseNoGenerator, DbCaseNoGenerator>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .SetIsOriginAllowed(_ => true) // TEMP: accept all origins
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddHttpClient<IPanicRealtime, PanicRealtimeMobileAdapter>((sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Realtime:BaseUrl"]
        ?? throw new InvalidOperationException("Realtime:BaseUrl missing");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

builder.Services.AddHttpClient<IOrderRealTime, OrderRealtimeMobileAdapter>((sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Realtime:BaseUrl"]
        ?? throw new InvalidOperationException("Realtime:BaseUrl missing");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

//Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Return proper 429 instead of default 503
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Unified JSON structure for rejections
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ra)
            ? ra.TotalSeconds.ToString("0")
            : null;

        var payload = new
        {
            error = "Too many requests",
            detail = "Rate limit exceeded. Please try again later.",
            retryAfterSeconds = retryAfter
        };

        await context.HttpContext.Response.WriteAsJsonAsync(payload, cancellationToken: token);
    };

    // Anonymous limiter (for public endpoints like Login, Register, OTP)
    options.AddPolicy("AnonymousLimiter", httpContext =>
    {
        // Identify unique client by IP + route (ensures fairness per endpoint)
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = httpContext.Request.Path.Value?.ToLowerInvariant() ?? "unknown";
        var partitionKey = $"anon:{path}:{clientIp}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,                   // Max 10 requests
                Window = TimeSpan.FromMinutes(1),   // Per minute
                QueueLimit = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    // Global background limiter (for authenticated users or fallback)
    options.AddFixedWindowLimiter("GlobalFixed", cfg =>
    {
        cfg.PermitLimit = 100;                // 100 requests per minute per app instance
        cfg.Window = TimeSpan.FromMinutes(1);
        cfg.QueueLimit = 10;
        cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/property/swagger.json", "Property Module");
    c.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth Module");
    c.SwaggerEndpoint("/swagger/club/swagger.json", "Club Module");
    c.SwaggerEndpoint("/swagger/panic/swagger.json", "Panic Module");
    c.SwaggerEndpoint("/swagger/Laundry/swagger.json", "Laundry Module");
    c.SwaggerEndpoint("/swagger/GroundBooking/swagger.json", "Ground Booking Module");
    c.SwaggerEndpoint("/swagger/other/swagger.json", "Other Module");
});

app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            context.Response.AddApplicationError(error.Error.Message);
            await context.Response.WriteAsync(error.Error.Message);
        }
    });
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/wwwroot"
});

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.MapControllers().RequireRateLimiting("GlobalFixed");


app.MapHub<VehicleLocationHub>("/hubs/vehicle-location");


//app.MapHub<PanicHub>("/hubs/panic");
app.UseMiddleware<CustomExceptionMiddleware>();
app.MapHealthChecks("/health");
app.UseRateLimiter();
app.Run();

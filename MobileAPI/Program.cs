using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Infrastructure.Data;
using DHAFacilitationAPIs.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MobileAPI;
using MobileAPI.Infrastructure;
using MobileAPI.RealTime;
using MobileAPI.Services;
using StackExchange.Redis;


var options = new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "wwwroot" //ensure this matches your actual web root folder
};

var builder = WebApplication.CreateBuilder(options);


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

// (E) JWT auth (also enable token from query string for WebSockets on the hub path)
//builder.Services
//    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        // TODO: set your token validation parameters here if not already set in Infrastructure:
//        // options.TokenValidationParameters = ...

//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                // This enables SignalR WebSocket auth: ?access_token=...
//                var token = context.Request.Query["access_token"];
//                var path = context.HttpContext.Request.Path;
//                if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs/panic"))
//                {
//                    context.Token = token;
//                }
//                return Task.CompletedTask;
//            }
//        };
//    });

//builder.Services.AddAuthorization();

// (F) Host-specific realtime adapter (Mobile)
builder.Services.AddScoped<IPanicRealtime, PanicRealtimeMobileAdapter>();
builder.Services.AddScoped<ICaseNoGenerator, DbCaseNoGenerator>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

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

app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.MapHub<PanicHub>("/hubs/panic").RequireAuthorization();

app.UseMiddleware<CustomExceptionMiddleware>();
app.Run();

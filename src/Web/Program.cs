using System.Net;
using System.Text;
using DHAFacilitationAPIs.Application;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Infrastructure;
using DHAFacilitationAPIs.Infrastructure.Data;
using DHAFacilitationAPIs.Infrastructure.Service;
using DHAFacilitationAPIs.Web;
using DHAFacilitationAPIs.Web.Infrastructure;
using DHAFacilitationAPIs.Web.RealTime;
using DHAFacilitationAPIs.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
// *** ADD: SignalR and your host adapter
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

builder.Services.AddSingleton<DapperConnectionFactory>();

builder.Services.AddHttpClient<ISmsService, SmsService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddControllers();

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();

builder.Services.Configure<SmartPayOptions>(builder.Configuration.GetSection("SmartPay"));
builder.Services.AddScoped<ISmartPayService, SmartPayService>();
builder.Services.AddScoped<IPropertyProcedureRepository, PropertyProcedureRepository>();
builder.Services.AddScoped<IMemberRenewalProcedurerespository, MemberDetailForRenewalProcedureRepository>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new ConfigurationOptions
    {
        EndPoints = { "172.16.10.123:6379" },
        Password = "DFP@234&Done",
        ConnectTimeout = 5000,  // optional: increase timeout
        AbortOnConnectFail = false
    };
    options.InstanceName = "RBAC_";
});
builder.Services.AddMemoryCache(); // backup fallback
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
builder.Services.AddScoped<IPermissionCache, RedisPermissionCache>();


// *** ADD: SignalR + Redis backplane
var redisConn = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddSignalR();

// *** ADD: Ensure JWT also works for SignalR WebSockets (token via ?access_token=)
builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, o =>
{
    var original = o.Events;
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Preserve existing handler if any
            if (original?.OnMessageReceived != null)
                return original.OnMessageReceived(context);

            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/panic"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// *** ADD: Host-specific realtime adapter (overrides any previous IPanicRealtime registration)
builder.Services.AddScoped<IPanicRealtime, PanicRealtimeWebAdapter>();
builder.Services.AddScoped<IOrderRealTime, OrderRealtimeWebAdapter>();
builder.Services.AddScoped<ICaseNoGenerator, DbCaseNoGenerator>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("CorsPolicy", b => b
//        .WithOrigins(
//            "http://localhost:3000",
//            "https://dfpwebapi.dhakarachi.org",
//            "https://gw.dhakarachi.org",
//            "http://172.16.10.123:3000",
//            "https://dfp.dhakarachi.org",
//            "http://192.168.5.121:3000"
//        )
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials()
//    );
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b =>
        b.SetIsOriginAllowed(_ => true) // allow any origin dynamically
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
    );
});



builder.Services.Configure<FirebaseSettings>(
    builder.Configuration.GetSection("FirebaseSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
   
}


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/CBMS/swagger.json", "CBMS Module");
    c.SwaggerEndpoint("/swagger/property/swagger.json", "Property Module");
    c.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth Module");
    c.SwaggerEndpoint("/swagger/club/swagger.json", "Club Module");
    c.SwaggerEndpoint("/swagger/panic/swagger.json", "Panic Module");
    c.SwaggerEndpoint("/swagger/laundry/swagger.json", "Laundry Module");
    c.SwaggerEndpoint("/swagger/Ground/swagger.json", "Ground Module");
    c.SwaggerEndpoint("/swagger/Femugation/swagger.json", "Femugation Module");
    c.SwaggerEndpoint("/swagger/RealTime-SignalR/swagger.json", "RealTime SignalR");
    c.SwaggerEndpoint("/swagger/Non-Member/swagger.json", "Non Member");
    c.SwaggerEndpoint("/swagger/MemberShip/swagger.json", "MemberShip");
    c.SwaggerEndpoint("/swagger/payment/swagger.json", "Payment Bills");
});

//app.UseHealthChecks("/health");



app.MapFallbackToFile("index.html");

//app.UseExceptionHandler(options => { });
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
        Path.Combine(app.Environment.ContentRootPath, "CBMS")),
    RequestPath = "/CBMS"
});
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

// *** ADD: Map the hub (path must match MobileApi)
app.MapHub<PanicHub>("/hubs/panic").RequireAuthorization();
app.MapHub<OrderHub>("/hubs/order").RequireAuthorization();

app.UseMiddleware<CustomExceptionMiddleware>();

app.Run();

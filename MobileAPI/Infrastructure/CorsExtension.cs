namespace MobileAPI.Infrastructure;

public static class CorsExtension
{
    public static void AddCustomCors(this IServiceCollection services)
    {
        //services.AddCors(options =>
        //{
        //    options.AddPolicy("CorsPolicy",
        //        builder => builder
        //            .AllowAnyOrigin()  // You can restrict this to specific origins if needed
        //            .AllowAnyMethod()
        //            .AllowAnyHeader());
        //});

        services.AddCors(o =>
  o.AddPolicy("CorsPolicy", b => b
    .WithOrigins("http://localhost:3000", "http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

    }
}

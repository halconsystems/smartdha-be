namespace DHAFacilitationAPIs.Web.Infrastructure;

public static class CorsExtension
{
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()  // You can restrict this to specific origins if needed
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        //services.AddCors(options =>
        //{
        //    options.AddPolicy("CorsPolicy", 
        //        b => b
        //        .WithOrigins(
        //    "http://localhost:3000",
        //    "http://localhost:5173",
        //    "https://dfpwebapi.dhakarachi.org",
        //    "https://gw.dhakarachi.org",   // add your frontend origins too if different
        //    "https://dfp.dhakarachi.org"
        //)
        //        .AllowAnyHeader()
        //        .AllowAnyMethod()
        //        .AllowCredentials());
        //});
    }
}

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
    }
}

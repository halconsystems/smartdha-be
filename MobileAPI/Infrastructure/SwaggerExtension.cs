using Microsoft.OpenApi.Models;

namespace MobileAPI.Infrastructure
{
    public static class SwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            #region Swagger

            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security",
            };

            var securityReq = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};


            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "BackendAPI - Test",
                Description = "Implementing",
            };


            #endregion



            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", info);
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });
        }
    }
}

using Microsoft.OpenApi.Models;

namespace DHAFacilitationAPIs.Web.Infrastructure
{
    //    public static class SwaggerExtension
    //    {
    //        public static void AddSwagger(this IServiceCollection services)
    //        {
    //            #region Swagger

    //            var securityScheme = new OpenApiSecurityScheme()
    //            {
    //                Name = "Authorization",
    //                Type = SecuritySchemeType.ApiKey,
    //                Scheme = "Bearer",
    //                BearerFormat = "JWT",
    //                In = ParameterLocation.Header,
    //                Description = "JSON Web Token based security",
    //            };

    //            var securityReq = new OpenApiSecurityRequirement()
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new string[] {}
    //    }
    //};


    //            var info = new OpenApiInfo()
    //            {
    //                Version = "v1",
    //                Title = "BackendAPI",
    //                Description = "Implementing",
    //            };


    //            #endregion



    //            services.AddEndpointsApiExplorer();
    //            services.AddSwaggerGen(o =>
    //            {
    //                o.SwaggerDoc("v1", info);
    //                o.AddSecurityDefinition("Bearer", securityScheme);
    //                o.AddSecurityRequirement(securityReq);
    //            });
    //        }
    //    }


    public static class SwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            #region Security

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Bearer token"
            };

            var securityReq = new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            };

            #endregion

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(o =>
            {
                // 🔹 MODULE-WISE SWAGGER DOCS
                o.SwaggerDoc("auth", new OpenApiInfo
                {
                    Title = "Auth Module",
                    Version = "v1"
                });

                o.SwaggerDoc("property", new OpenApiInfo
                {
                    Title = "Property Module",
                    Version = "v1"
                });

                o.SwaggerDoc("club", new OpenApiInfo
                {
                    Title = "Club Module",
                    Version = "v1"
                });
                o.SwaggerDoc("CBMS", new OpenApiInfo
                {
                    Title = "CBMS Module",
                    Version = "v1"
                });

                o.SwaggerDoc("panic", new OpenApiInfo
                {
                    Title = "Panic Module",
                    Version = "v1"
                });

                o.SwaggerDoc("laundry", new OpenApiInfo
                {
                    Title = "Laudry Module",
                    Version = "v1"
                });
                o.SwaggerDoc("Femugation", new OpenApiInfo
                {
                    Title = "Femugation Module",
                    Version = "v1"
                });
                o.SwaggerDoc("Ground", new OpenApiInfo
                {
                    Title = "Ground Module",
                    Version = "v1"
                });
                o.SwaggerDoc("RealTime-SignalR", new OpenApiInfo
                {
                    Title = "RealTime SignalR",
                    Version = "v1"
                });
                o.SwaggerDoc("Non-Member", new OpenApiInfo
                {
                    Title = "Non Member",
                    Version = "v1"
                });
                o.SwaggerDoc("MemberShip", new OpenApiInfo
                {
                    Title = "MemberShip",
                    Version = "v1"
                });

                // 🔹 FILTER APIs BY GROUP NAME
                o.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (string.IsNullOrWhiteSpace(apiDesc.GroupName))
                        return false;

                    return apiDesc.GroupName.Equals(docName, StringComparison.OrdinalIgnoreCase);
                });

                // 🔹 JWT
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });
        }

    }
}

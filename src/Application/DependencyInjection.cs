using System.Reflection;
using DHAFacilitationAPIs.Application.Interface;
using DHAFacilitationAPIs.Application.Common.Behaviours;
using DHAFacilitationAPIs.Application.Common.DependencyResolver;
using DHAFacilitationAPIs.Application.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DHAFacilitationAPIs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        services.AutoDependencyResolverServices();


        #region Get Section Settings

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        #endregion

        return services;
    }
}

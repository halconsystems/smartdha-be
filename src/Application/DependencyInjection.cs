using System.Reflection;
using DHAFacilitationAPIs.Application;
using DHAFacilitationAPIs.Application.Common.Behaviours;
using DHAFacilitationAPIs.Application.Common.DependencyResolver;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Application.Interface;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appAssembly = typeof(ApplicationAssemblyMarker).Assembly;

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(appAssembly);
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            cfg.RegisterServicesFromAssembly(typeof(GetDropdownQueryHandler<>).Assembly);
        });


        // 🔒 Explicitly register closed generics for each entity you need as a dropdown
        RegisterDropdown<Club>(services);
        RegisterDropdown<RoomCategory>(services);
        RegisterDropdown<ResidenceType>(services);
        RegisterDropdown<Services>(services);
        // ...add more here


        services.AutoDependencyResolverServices();


        #region Get Section Settings

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<FirebaseOptions>(configuration.GetSection(FirebaseOptions.SectionName));

        #endregion

        return services;
    }

    private static void RegisterDropdown<TEntity>(IServiceCollection services) where TEntity : class
    {
        var requestType = typeof(GetDropdownQuery<>).MakeGenericType(typeof(TEntity));
        var responseType = typeof(List<DropdownDto>);
        var serviceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var implType = typeof(GetDropdownQueryHandler<>).MakeGenericType(typeof(TEntity));

        services.AddTransient(serviceType, implType);
    }
}

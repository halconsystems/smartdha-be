using Microsoft.Extensions.DependencyInjection;

namespace DHAFacilitationAPIs.Application.Common.DependencyResolver;

public static class DependenciesResolver
{
    internal static IServiceCollection AutoDependencyResolverServices(this IServiceCollection services)
    {
        return services
           .ScanServices(typeof(IServicesType.ISingletonService), ServiceLifetime.Singleton)
           .ScanServices(typeof(IServicesType.ITransientService), ServiceLifetime.Transient)
           .ScanServices(typeof(IServicesType.IScopedService), ServiceLifetime.Scoped);
    }
    internal static IServiceCollection ScanServices(this IServiceCollection services, Type interfaceType, ServiceLifetime servicelifetime)
    {
        var interfaceTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => interfaceType.IsAssignableFrom(t)
                            && t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterfaces().FirstOrDefault(),
                    Implementation = t
                })
                .Where(t => t.Service is not null
                            && interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            services.RegisterServices(type.Service!, type.Implementation, servicelifetime);
        }

        return services;
    }

    internal static IServiceCollection RegisterServices(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(serviceLifetime))
        };
    }
}

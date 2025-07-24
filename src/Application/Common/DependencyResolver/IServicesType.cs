namespace DHAFacilitationAPIs.Application.Common.DependencyResolver;

public interface IServicesType
{
    public interface IScopedService { }
    public interface ISingletonService { }
    public interface ITransientService { }
}

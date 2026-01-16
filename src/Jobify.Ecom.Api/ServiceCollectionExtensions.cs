namespace Jobify.Ecom.Api;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiServices()
        {
            return services;
        }
    }
}

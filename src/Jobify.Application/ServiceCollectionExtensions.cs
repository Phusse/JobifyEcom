using Jobify.Application.Configurations.Security;
using Jobify.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices(IConfiguration configuration)
        {
            services.Configure<SessionManagementOptions>(configuration.GetSection("SessionManagement"));
            services.AddScoped<SessionManagementService>();

            return services;
        }
    }
}

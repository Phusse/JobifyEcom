using Jobify.Ecom.Persistence.Context;
using Jobify.Ecom.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Ecom.Persistence;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPersistenceServices(IConfiguration configuration, AppEnvironment appEnvironment)
        {
            string connectionName = appEnvironment switch
            {
                AppEnvironment.Local => "localdatabase",
                AppEnvironment.Test => "testdatabase",
                AppEnvironment.Production => "productiondatabase",
                _ => throw new ArgumentOutOfRangeException(nameof(appEnvironment), "Unsupported application environment")
            };

            string connectionString = configuration.GetConnectionString(connectionName)
                ?? throw new InvalidOperationException($"Connection string '{connectionName}' is not configured.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            return services;
        }
    }
}

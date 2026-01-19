using System.Reflection;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Infrastructure.CQRS.Messaging;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Jobify.Ecom.Infrastructure.CQRS.Decorators;
using Jobify.Ecom.Application.CQRS.Decorators;
using Jobify.Ecom.Application.Services;
using Jobify.Ecom.Infrastructure.Services;

namespace Jobify.Ecom.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(IConfiguration configuration, params Assembly[] assembliesToScan)
        {
            services.AddSingleton<ICacheService, RedisCacheService>();

            services.AddCqrsWithValidation(assembliesToScan);

            return services;
        }

        private void AddCqrsWithValidation(Assembly[] assembliesToScan)
        {
            services.AddScoped<IMediator, Mediator>();

            foreach (Assembly assembly in assembliesToScan)
            {
                services.Scan(scan => scan
                    .FromAssemblies(assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                );

                services.Scan(scan => scan
                    .FromAssemblies(assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                );
            }

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}

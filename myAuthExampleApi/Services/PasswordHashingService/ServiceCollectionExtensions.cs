using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.PasswordHashingService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPasswordHashingService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IPasswordHashingService, PasswordHashingService>();
            return serviceCollection;
        }

        public static IServiceCollection AddPasswordHashingService(this IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.AddScoped<IPasswordHashingService, PasswordHashingService>();
            serviceCollection.Configure<PasswordHashingSettings>(config);
            return serviceCollection;
        }

        public static IServiceCollection AddPasswordHashingService(this IServiceCollection serviceCollection, Action<PasswordHashingSettings> options)
        {
            serviceCollection.AddScoped<IPasswordHashingService, PasswordHashingService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }

        public static IServiceCollection AddPasswordHashingService(this IServiceCollection serviceCollection, IConfiguration config = null, Action<PasswordHashingSettings> options = null)
        {
            serviceCollection.AddScoped<IPasswordHashingService, PasswordHashingService>();
            if (config != null) serviceCollection.Configure<PasswordHashingSettings>(config);
            if (options != null) serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}

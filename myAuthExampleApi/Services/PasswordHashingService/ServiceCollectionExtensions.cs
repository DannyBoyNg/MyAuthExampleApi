using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.PasswordHashingServ
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
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.SimpleTokenServ
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleTokenService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISimpleTokenService, SimpleTokenService>();
            return serviceCollection;
        }

        public static IServiceCollection AddSimpleTokenService(this IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.AddScoped<ISimpleTokenService, SimpleTokenService>();
            serviceCollection.Configure<SimpleTokenSettings>(config);
            return serviceCollection;
        }

        public static IServiceCollection AddSimpleTokenService(this IServiceCollection serviceCollection, Action<SimpleTokenSettings> options)
        {
            serviceCollection.AddScoped<ISimpleTokenService, SimpleTokenService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}

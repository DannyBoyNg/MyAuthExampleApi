using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.JwtTokenService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtTokenService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IJwtTokenService, JwtTokenService>();
            return serviceCollection;
        }

        public static IServiceCollection AddJwtTokenService(this IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.AddScoped<IJwtTokenService, JwtTokenService>();
            serviceCollection.Configure<JwtTokenSettings>(config);
            return serviceCollection;
        }

        public static IServiceCollection AddJwtTokenService(this IServiceCollection serviceCollection, Action<JwtTokenSettings> options)
        {
            serviceCollection.AddScoped<IJwtTokenService, JwtTokenService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }

        public static IServiceCollection AddJwtTokenService(this IServiceCollection serviceCollection, IConfiguration config = null, Action<JwtTokenSettings> options = null)
        {
            serviceCollection.AddScoped<IJwtTokenService, JwtTokenService>();
            if (config != null) serviceCollection.Configure<JwtTokenSettings>(config);
            if (options != null) serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}

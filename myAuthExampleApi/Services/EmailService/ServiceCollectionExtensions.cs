using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.EmailService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            return serviceCollection;
        }

        public static IServiceCollection AddEmailService(this IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.Configure<EmailSettings>(config);
            return serviceCollection;
        }

        public static IServiceCollection AddEmailService(this IServiceCollection serviceCollection, Action<EmailSettings> options)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }

        public static IServiceCollection AddEmailService(this IServiceCollection serviceCollection, IConfiguration config = null, Action<EmailSettings> options = null)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            if (config != null) serviceCollection.Configure<EmailSettings>(config);
            if (options != null) serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.EmailServ
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            return serviceCollection;
        }

        public static IServiceCollection AddEmailService(this IServiceCollection serviceCollection, Action<EmailSettings> options)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Services.UserService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserService, UserService>();
            return serviceCollection;
        }
    }
}

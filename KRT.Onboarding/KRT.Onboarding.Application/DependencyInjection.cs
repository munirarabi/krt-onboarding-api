using KRT.Onboarding.Application.Interfaces.Services;
using KRT.Onboarding.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KRT.Onboarding.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
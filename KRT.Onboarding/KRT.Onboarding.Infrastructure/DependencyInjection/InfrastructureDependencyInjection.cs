using KRT.Onboarding.Application.Interfaces.Caching;
using KRT.Onboarding.Application.Interfaces.Messaging;
using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Infrastructure.Caching;
using KRT.Onboarding.Infrastructure.Messaging;
using KRT.Onboarding.Infrastructure.Persistence.Context;
using KRT.Onboarding.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KRT.Onboarding.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // obtém as connection strings do SQL Server e Redis
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var redisConnectionString = configuration.GetConnectionString("Redis");

        // registra o DbContext utilizando SQL Server
        services.AddDbContext<OnboardingDbContext>(
            options => options.UseSqlServer(connectionString));

        // registra a implementação do repositório
        services.AddScoped<IAccountRepository, AccountRepository>();

        // configura o Redis como implementacao do cache distribuído
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        // registra a implementacao do serviço de cache
        services.AddScoped<IAccountCacheService, RedisAccountCacheService>();

        // Registra a implementação responsável pela publicação dos eventos
        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}
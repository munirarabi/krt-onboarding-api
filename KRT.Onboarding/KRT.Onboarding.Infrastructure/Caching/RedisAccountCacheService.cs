using System.Text.Json;
using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace KRT.Onboarding.Infrastructure.Caching
{
    public class RedisAccountCacheService : IAccountCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisAccountCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<AccountDto?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var key = GetKey(id);

            var cachedAccount = await _cache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrWhiteSpace(cachedAccount))
            {
                return null;
            }

            return JsonSerializer.Deserialize<AccountDto>(cachedAccount);
        }

        public async Task SetAsync(AccountDto account, CancellationToken cancellationToken)
        {
            var key = GetKey(account.Id);

            var value = JsonSerializer.Serialize(account);

            var options = new DistributedCacheEntryOptions
            {
                // defini 1 dia de duração para o dado ficar armazenado em cache
                // o desafio pede especificamente em evitar consultas repetidas para uma conta que foi consultada naquele mesmo dia
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // TTL de 24 horas.
            };

            await _cache.SetStringAsync(key, value, options, cancellationToken);
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(GetKey(id), cancellationToken);
        }

        private static string GetKey(Guid id)
        {
            return $"account:{id}";
        }
    }
}
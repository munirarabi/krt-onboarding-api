using KRT.Onboarding.Application.DTOs;

namespace KRT.Onboarding.Application.Interfaces.Caching
{
    public interface IAccountCacheService
    {
        Task<AccountDto?> GetAsync(Guid id, CancellationToken cancellationToken);
        Task SetAsync(AccountDto account, CancellationToken cancellationToken);
        Task RemoveAsync(Guid id, CancellationToken cancellationToken);
    }
}
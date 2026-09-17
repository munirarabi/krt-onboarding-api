using KRT.Onboarding.Domain.Entities;

namespace KRT.Onboarding.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken);
        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken);
        Task AddAsync(Account account, CancellationToken cancellationToken);
        Task UpdateAsync(Account account, CancellationToken cancellationToken);
        Task DeleteAsync(Account account, CancellationToken cancellationToken);
    }
}

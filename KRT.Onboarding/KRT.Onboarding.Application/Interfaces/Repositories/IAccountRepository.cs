using KRT.Onboarding.Domain.Entities;

namespace KRT.Onboarding.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task<IEnumerable<Account>> GetAllAsync();
        Task<bool> ExistsByCpfAsync(string cpf);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
    }
}

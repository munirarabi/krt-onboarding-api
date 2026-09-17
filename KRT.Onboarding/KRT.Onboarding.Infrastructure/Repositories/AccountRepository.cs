using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Infrastructure.Persistence.Context;

namespace KRT.Onboarding.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly OnboardingDbContext _context;

        public AccountRepository(OnboardingDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByCpfAsync(string cpf)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Account?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}

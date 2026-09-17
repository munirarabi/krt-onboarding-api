using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.ValueObjects;
using KRT.Onboarding.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace KRT.Onboarding.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly OnboardingDbContext _context;

        public AccountRepository(OnboardingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Accounts.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken)
        {
            var cpfValue = new Cpf(cpf);

            return await _context.Accounts.AnyAsync(x => x.Cpf == cpfValue, cancellationToken);
        }

        public async Task AddAsync(Account account, CancellationToken cancellationToken)
        {
            await _context.Accounts.AddAsync(account, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Account account, CancellationToken cancellationToken)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Account account, CancellationToken cancellationToken)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
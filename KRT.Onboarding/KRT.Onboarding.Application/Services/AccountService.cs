using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Application.Interfaces.Services;

namespace KRT.Onboarding.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public Task<AccountDto> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AccountDto> GetByIdAsync()
        {
            throw new NotImplementedException();
        }
    }
}

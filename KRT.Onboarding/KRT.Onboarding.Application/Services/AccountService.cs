using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Application.Interfaces.Services;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDto> CreateAsync(string holderName, string cpf, CancellationToken cancellationToken)
        {
            var cpfValue = new Cpf(cpf);

            var exists = await _accountRepository.ExistsByCpfAsync(cpfValue.Value, cancellationToken);

            if (exists)
                throw new ConflictException("An account with this CPF already exists.");

            var account = new Account(holderName, cpfValue);

            await _accountRepository.AddAsync(account, cancellationToken);

            return MapToDto(account);
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var accounts = await _accountRepository.GetAllAsync(cancellationToken);

            return accounts.Select(MapToDto);
        }

        public async Task<AccountDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
                throw new NotFoundException($"Account with ID '{id}' was not found.");

            return MapToDto(account);
        }

        public async Task<AccountDto> UpdateAsync(Guid id, string holderName, AccountStatus status, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
                throw new NotFoundException($"Account with ID '{id}' was not found.");

            account.UpdateHolderName(holderName);
            account.ChangeStatus(status);

            await _accountRepository.UpdateAsync(account, cancellationToken);

            return MapToDto(account);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
                throw new NotFoundException($"Account with ID '{id}' was not found.");

            await _accountRepository.DeleteAsync(account, cancellationToken);
        }

        private static AccountDto MapToDto(Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
                HolderName = account.HolderName,
                Cpf = account.Cpf.Value,
                Status = account.Status
            };
        }
    }
}

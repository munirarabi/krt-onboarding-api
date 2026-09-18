using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Application.Interfaces.Caching;
using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Application.Interfaces.Services;
using KRT.Onboarding.Application.Mappings;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.Application.Services
{
    public class AccountService : IAccountService
    {
        #region injeção de dependência
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountCacheService _accountCacheService;


        public AccountService(IAccountRepository accountRepository,
                              IAccountCacheService accountCacheService)
        {
            _accountRepository = accountRepository;
            _accountCacheService = accountCacheService;
        }
        #endregion

        public async Task<AccountDto> CreateAsync(string holderName,
                                                  string cpf,
                                                  CancellationToken cancellationToken)
        {
            var cpfValue = new Cpf(cpf);
            var holderNameValue = new HolderName(holderName);

            var exists = await _accountRepository.ExistsByCpfAsync(cpfValue.Value, cancellationToken);

            if (exists)
            {
                throw new ConflictException("An account with this CPF already exists.");
            }

            var account = new Account(holderNameValue, cpfValue);

            await _accountRepository.AddAsync(account, cancellationToken);

            return AccountMapping.ToDto(account);
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Account> accounts = await _accountRepository.GetAllAsync(cancellationToken);

            return accounts.Select(AccountMapping.ToDto);
        }

        // Metodo utilizando comportamento de cache-aside:
        public async Task<AccountDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            // Primeiro pesquisa o dado no cache
            var cachedAccount = await _accountCacheService.GetAsync(id, cancellationToken);

            // Se existir no cache retorna ele.
            if (cachedAccount is not null)
            {
                return cachedAccount;
            }

            // Se não existir no cache, busca no banco.
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
            {
                throw new NotFoundException($"Account with ID '{id}' was not found.");
            }

            var accountDto = AccountMapping.ToDto(account);

            await _accountCacheService.SetAsync(accountDto, cancellationToken);

            return accountDto;
        }

        // Metodo de update utilizando invalidacao de cache
        public async Task<AccountDto> UpdateAsync(Guid id,
                                                  string holderName,
                                                  AccountStatus status,
                                                  CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
            {
                throw new NotFoundException($"Account with ID '{id}' was not found.");
            }

            var holderNameValue = new HolderName(holderName);

            account.UpdateHolderName(holderNameValue);
            account.ChangeStatus(status);

            await _accountRepository.UpdateAsync(account, cancellationToken);

            await _accountCacheService.RemoveAsync(id, cancellationToken);

            return AccountMapping.ToDto(account);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
            {
                throw new NotFoundException($"Account with ID '{id}' was not found.");
            }

            await _accountRepository.DeleteAsync(account, cancellationToken);

            await _accountCacheService.RemoveAsync(id, cancellationToken);
        }
    }
}

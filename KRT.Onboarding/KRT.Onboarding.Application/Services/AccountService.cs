using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Application.Interfaces.Caching;
using KRT.Onboarding.Application.Interfaces.Messaging;
using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Application.Interfaces.Services;
using KRT.Onboarding.Application.Mappings;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Events;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KRT.Onboarding.Application.Services
{
    public class AccountService : IAccountService
    {
        #region injeção de dependência
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountCacheService _accountCacheService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository,
                              IAccountCacheService accountCacheService,
                              IEventPublisher eventPublisher,
                              ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _accountCacheService = accountCacheService;
            _eventPublisher = eventPublisher;
            _logger = logger;
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

            try
            {
                // Publica o evento de criação da conta na mensageria
                await _eventPublisher.PublishAsync(
                    new AccountCreatedEvent(account.Id,
                                            account.HolderName.Value,
                                            account.Cpf.Value,
                                            DateTime.UtcNow),
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to publish AccountCreatedEvent for account {AccountId}.",
                    account.Id
                );

                // TODO: Implementar mecanismo de retry/fila para eventos nao publicados
            }

            return AccountMapping.ToDto(account);
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Account> accounts = await _accountRepository.GetAllAsync(cancellationToken);

            var accountsList = accounts.Select(AccountMapping.ToDto);

            return accountsList;
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

            account.Update(holderNameValue, status);

            await _accountRepository.UpdateAsync(account, cancellationToken);

            await _accountCacheService.RemoveAsync(id, cancellationToken);

            try
            {
                // Publica o evento de atualização da conta na mensageria
                await _eventPublisher.PublishAsync(
                    new AccountUpdatedEvent(
                        account.Id,
                        account.HolderName.Value,
                        account.Status.ToString(),
                        DateTime.UtcNow),
                    cancellationToken
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to publish AccountUpdatedEvent for account {AccountId}.",
                    account.Id
                );

                // TODO: implementar mecanismo de retry/fila para eventos não publicados
            }

            return AccountMapping.ToDto(account);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

            if (account is null)
            {
                throw new NotFoundException($"Account with ID '{id}' was not found.");
            }

            // Deleta do banco
            await _accountRepository.DeleteAsync(account, cancellationToken);

            // Deleta do cache
            await _accountCacheService.RemoveAsync(id, cancellationToken);

            try
            {
                // publica o evento de exclusão da conta na mensageria
                await _eventPublisher.PublishAsync(
                    new AccountDeletedEvent(
                        account.Id,
                        DateTime.UtcNow),
                    cancellationToken
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to publish AccountDeletedEvent for account {AccountId}.",
                    account.Id
                );

                // TODO: implementar mecanismo de retry/fila para eventos não publicados
            }
        }
    }
}

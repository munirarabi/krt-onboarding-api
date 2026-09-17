using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Domain.Enums;

namespace KRT.Onboarding.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AccountDto> CreateAsync(string holderName, string cpf, CancellationToken cancellationToken);

        Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken);

        Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<AccountDto?> UpdateAsync(Guid id, string holderName, AccountStatus status, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}

using KRT.Onboarding.Application.DTOs;

namespace KRT.Onboarding.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AccountDto> GetByIdAsync();
    }
}

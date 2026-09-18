using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Domain.Entities;

namespace KRT.Onboarding.Application.Mappings
{
    public static class AccountMapping
    {
        public static AccountDto ToDto(Account account)
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
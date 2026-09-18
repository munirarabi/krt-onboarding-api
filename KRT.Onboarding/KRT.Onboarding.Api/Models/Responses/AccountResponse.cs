using KRT.Onboarding.Application.DTOs;

namespace KRT.Onboarding.Api.Models.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public static AccountResponse FromDto(AccountDto account)
        {
            return new AccountResponse
            {
                Id = account.Id,
                HolderName = account.HolderName,
                Cpf = account.Cpf,
                Status = account.Status.ToString()
            };
        }
    }
}
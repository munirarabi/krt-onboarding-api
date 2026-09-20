using KRT.Onboarding.Application.DTOs;

namespace KRT.Onboarding.Api.Models.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static AccountResponse FromDto(AccountDto account)
        {
            return new AccountResponse
            {
                Id = account.Id,
                HolderName = account.HolderName,
                Cpf = account.Cpf,
                Status = account.Status.ToString(),
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }
    }
}
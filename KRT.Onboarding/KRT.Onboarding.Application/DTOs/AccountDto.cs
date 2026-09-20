using KRT.Onboarding.Domain.Enums;

namespace KRT.Onboarding.Application.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public AccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

using KRT.Onboarding.Domain.Enums;

namespace KRT.Onboarding.Api.Models.Requests
{
    public class UpdateAccountRequest
    {
        public string HolderName { get; set; } = string.Empty;
        public AccountStatus Status { get; set; }
    }
}

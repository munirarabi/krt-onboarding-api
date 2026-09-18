using System.ComponentModel.DataAnnotations;
using KRT.Onboarding.Domain.Enums;

namespace KRT.Onboarding.Api.Models.Requests
{
    public class UpdateAccountRequest
    {
        [Required]
        [MaxLength(150)]
        public string HolderName { get; set; } = string.Empty;
        [Required]
        public AccountStatus Status { get; set; }
    }
}
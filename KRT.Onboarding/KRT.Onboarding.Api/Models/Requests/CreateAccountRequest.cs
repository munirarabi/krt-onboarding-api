using System.ComponentModel.DataAnnotations;

namespace KRT.Onboarding.Api.Models.Requests
{
    public class CreateAccountRequest
    {
        [Required]
        [MaxLength(150)]
        public string HolderName { get; set; } = string.Empty;

        [Required]
        public string Cpf { get; set; } = string.Empty;
    }
}

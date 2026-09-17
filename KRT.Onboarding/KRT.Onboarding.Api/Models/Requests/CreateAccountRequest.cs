namespace KRT.Onboarding.Api.Models.Requests
{
    public class CreateAccountRequest
    {
        public string HolderName { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
    }
}

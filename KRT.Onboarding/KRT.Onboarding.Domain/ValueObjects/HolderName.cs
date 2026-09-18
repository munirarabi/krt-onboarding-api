using KRT.Onboarding.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace KRT.Onboarding.Domain.ValueObjects
{
    public sealed record HolderName
    {
        public string Value { get; }

        public HolderName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("Holder name cannot be empty.");
            }

            var normalizedName = Normalize(value);

            if (normalizedName.Length < 2)
            {
                throw new DomainException("Holder name must contain at least 2 characters.");
            }

            // Não precisaria pois na propria classe CreateAccountRequest e UpdateAccountRequest já existem essa validação
            if (normalizedName.Length > 150)
            {
                throw new DomainException("Holder name cannot exceed 150 characters.");
            }

            if (!normalizedName.Any(char.IsLetter))
            {
                throw new DomainException("Holder name must contain letters.");
            }

            if (normalizedName.Any(char.IsDigit))
            {
                throw new DomainException("Holder name cannot contain numbers.");
            }

            // Valida se o nome contem apenas letras, espaços e caracteres permitidos
            // permitidos são:  (', . e -)
            if (!Regex.IsMatch(normalizedName, @"^[\p{L}\s'.-]+$"))
            {
                throw new DomainException("Holder name contains invalid characters.");
            }

            Value = normalizedName;
        }

        private static string Normalize(string value)
        {
            return Regex.Replace(value.Trim(), @"\s+", " ");
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
using KRT.Onboarding.Domain.Exceptions;

namespace KRT.Onboarding.Domain.ValueObjects
{
    public sealed record Cpf
    {
        public string Value { get; }

        public Cpf(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("CPF cannot be empty.");
            }

            // remove máscara e mantém somente os números
            var normalizedCpf = new string(value.Where(char.IsDigit).ToArray());

            if (!IsValid(normalizedCpf))
            {
                throw new DomainException("Invalid CPF.");
            }

            Value = normalizedCpf;
        }

        private static bool IsValid(string cpf)
        {
            // CPF precisa ter exatamente 11 dígitos
            if (cpf.Length != 11)
                return false;

            // evita CPFs com todos os números iguais
            if (cpf.All(x => x == cpf[0]))
                return false;

            var numbers = cpf.Select(x => x - '0').ToArray();

            // cálculo do primeiro dígito verificador
            var sum = 0;

            for (var i = 0; i < 9; i++)
                sum += numbers[i] * (10 - i);

            var remainder = sum % 11;
            var firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if (numbers[9] != firstDigit)
                return false;

            // cálculo do segundo dígito verificador
            sum = 0;

            for (var i = 0; i < 10; i++)
                sum += numbers[i] * (11 - i);

            remainder = sum % 11;
            var secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return numbers[10] == secondDigit;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
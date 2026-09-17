namespace KRT.Onboarding.Domain.ValueObjects
{
    /*
     Cpf é um Value Object (Objeto de Valor) no contexto de DDD
     O sealed significa que ninguém pode herdar dessa classe
     */
    public sealed class Cpf
    {
        public string Value { get; }

        public Cpf(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CPF cannot be empty.");

            var normalizedCpf = Normalize(value);

            if (!IsValid(normalizedCpf))
                throw new ArgumentException("Invalid CPF.");

            Value = normalizedCpf;
        }

        private static string Normalize(string cpf)
        {
            // remover tudo que não for número do CPF
            return new string(cpf.Where(char.IsDigit).ToArray());
        }

        private static bool IsValid(string cpf)
        {
            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            return ValidateDigit(cpf, 9) &&
                   ValidateDigit(cpf, 10);
        }

        private static bool ValidateDigit(string cpf, int position)
        {
            var sum = 0;
            var weight = position + 1;

            for (var i = 0; i < position; i++)
            {
                sum += (cpf[i] - '0') * weight--;
            }

            var remainder = sum % 11;
            var digit = remainder < 2 ? 0 : 11 - remainder;

            return cpf[position] - '0' == digit;
        }

        //public override string ToString()
        //{
        //    return Value;
        //}
    }
}

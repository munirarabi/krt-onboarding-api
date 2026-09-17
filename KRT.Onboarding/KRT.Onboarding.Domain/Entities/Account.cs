using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string HolderName { get; private set; }
        public Cpf Cpf { get; private set; }
        public AccountStatus Status { get; private set; }

        public Account(string holderName, Cpf cpf)
        {
            ValidateHolderName(holderName);

            Id = Guid.NewGuid();
            HolderName = holderName.Trim();
            Cpf = cpf;
            Status = AccountStatus.Active; // por padrão os novos usuários começaram como Ativos.
        }

        public void UpdateHolderName(string holderName)
        {
            ValidateHolderName(holderName);

            HolderName = holderName.Trim();
        }

        public void ChangeStatus(AccountStatus status)
        {
            if (!Enum.IsDefined(status))
            {
                throw new DomainException("Invalid account status");
            }

            Status = status;
        }

        private static void ValidateHolderName(string holderName)
        {
            if (string.IsNullOrWhiteSpace(holderName))
            {
                throw new DomainException("Holder name cannot be empty");
            }

            if (holderName.Length > 150)
            {
                throw new DomainException("Holder name cannot exceed 150 characters");
            }

            if (holderName.Length < 3)
            {
                throw new DomainException("Holder name must be at least 3 characters long.");
            }
        }
    }
}
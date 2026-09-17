using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string HolderName { get; private set; }
        public Cpf Cpf { get; private set; }
        public AccountStatus Status { get; private set; }

        private Account() { }

        public Account(
            string holderName,
            Cpf cpf)
        {
            if (string.IsNullOrWhiteSpace(holderName))
                throw new ArgumentException("Holder name cannot be empty.");

            Id = Guid.NewGuid();
            HolderName = holderName;
            Cpf = cpf;
            Status = AccountStatus.Active;
        }

        public void UpdateHolderName(string holderName)
        {
            if (string.IsNullOrWhiteSpace(holderName))
                throw new ArgumentException("Holder name cannot be empty.");

            HolderName = holderName;
        }

        public void Activate()
        {
            Status = AccountStatus.Active;
        }

        public void Deactivate()
        {
            Status = AccountStatus.Inactive;
        }
    }
}

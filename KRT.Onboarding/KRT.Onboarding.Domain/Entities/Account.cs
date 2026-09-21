using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public HolderName HolderName { get; private set; }
        public Cpf Cpf { get; private set; }
        public AccountStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Account()
        {
            HolderName = null!;
            Cpf = null!;
        }

        public Account(HolderName holderName, Cpf cpf)
        {
            Id = Guid.NewGuid();
            HolderName = holderName;
            Cpf = cpf;
            Status = AccountStatus.Active; // novos usuários irão começar como ativos por default.
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(HolderName holderName, AccountStatus status)
        {
            if (!Enum.IsDefined(status))
            {
                throw new DomainException("Invalid account status");
            }

            HolderName = holderName;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Inactivate()
        {
            Status = AccountStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
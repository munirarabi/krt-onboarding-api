namespace KRT.Onboarding.Domain.Events
{
    public record AccountCreatedEvent(Guid AccountId, string HolderName, string Cpf, DateTime OccurredAt);
}

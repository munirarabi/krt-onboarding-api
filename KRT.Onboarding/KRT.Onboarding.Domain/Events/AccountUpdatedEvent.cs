namespace KRT.Onboarding.Domain.Events
{
    public record AccountUpdatedEvent(Guid AccountId, string HolderName, string Status, DateTime OccurredAt);
}

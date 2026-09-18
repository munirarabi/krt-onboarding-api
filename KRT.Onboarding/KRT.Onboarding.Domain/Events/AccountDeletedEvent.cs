namespace KRT.Onboarding.Domain.Events
{
    public record AccountDeletedEvent(Guid AccountId, DateTime OccurredAt);
}

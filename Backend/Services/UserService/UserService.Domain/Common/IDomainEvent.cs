namespace UserService.Domain.Common;

public interface IDomainEvent
{
    public DateTime OccurredOn { get; }
}
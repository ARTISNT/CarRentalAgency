namespace UserService.Infrastructure.EmailOutbox;

public class OutboxOptions
{
    public const string SectionName = "Outbox";

    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 50;
    public int MaxAttempts { get; set; } = 5;
    public int LockMinutes { get; set; } = 5;
}

using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions;
using UserService.Application.EmailOutbox;

namespace UserService.Infrastructure.Repositories;

public interface IOutboxReader
{
    Task<List<EmailOutboxEntry>> ClaimPendingAsync(
        DateTime now,
        int batchSize,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default);

    Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default);

    Task MarkFailedAsync(Guid id, int attempts, string lastError, DateTime nextAttemptAt, CancellationToken cancellationToken = default);
}

public class EmailOutboxRepository(UserServiceContext context)
    : IEmailOutboxRepository, IOutboxReader
{
    public void Add(EmailOutboxEntry entry)
    {
        context.EmailOutbox.Add(entry);
    }

    public async Task<List<EmailOutboxEntry>> ClaimPendingAsync(
        DateTime now,
        int batchSize,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default)
    {
        var lockUntil = now.Add(lockDuration);

        var pending = await context.EmailOutbox
            .FromSqlRaw(
                @"SELECT TOP({0}) * FROM email_outbox WITH (READPAST, UPDLOCK)
                  WHERE processed_at IS NULL AND next_attempt_at <= {1}
                  ORDER BY next_attempt_at",
                batchSize, now)
            .ToListAsync(cancellationToken);

        if (pending.Count == 0)
            return pending;

        var ids = pending.Select(p => p.Id).ToList();

        var parameters = new List<object> { lockUntil };
        var idPlaceholders = string.Join(",", ids.Select((_, i) =>
        {
            parameters.Add(ids[i]);
            return $"{{{i + 1}}}";
        }));

        await context.Database.ExecuteSqlRawAsync(
            $@"UPDATE email_outbox
              SET next_attempt_at = {{0}}
              WHERE id IN ({idPlaceholders})",
            parameters.ToArray()!,
            cancellationToken);

        foreach (var entry in pending)
            entry.NextAttemptAt = lockUntil;

        return pending;
    }

    public async Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await context.Database.ExecuteSqlRawAsync(
            "UPDATE email_outbox SET processed_at = SYSUTCDATETIME(), last_error = NULL WHERE id = {0}",
            new object[] { id },
            cancellationToken);
    }

    public async Task MarkFailedAsync(Guid id, int attempts, string lastError, DateTime nextAttemptAt, CancellationToken cancellationToken = default)
    {
        var truncated = lastError.Length > 2000 ? lastError[..2000] : lastError;
        await context.Database.ExecuteSqlRawAsync(
            "UPDATE email_outbox SET attempts = {0}, next_attempt_at = {1}, last_error = {2} WHERE id = {3}",
            new object[] { attempts, nextAttemptAt, truncated, id },
            cancellationToken);
    }
}

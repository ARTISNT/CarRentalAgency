using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace RentalService.Api.Common;

public static class MigrationRunner
{
    public static async Task RunWithRetryAsync(
        DbContext db,
        ILogger logger,
        int maxAttempts = 6,
        CancellationToken cancellationToken = default)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await db.Database.MigrateAsync(cancellationToken);
                if (attempt > 1)
                {
                    logger.LogInformation(
                        "Database migration succeeded on attempt {Attempt}",
                        attempt);
                }
                return;
            }
            catch (SqlException ex) when (IsTransient(ex) || ex.Number == 1801)
            {
                if (attempt == maxAttempts)
                {
                    logger.LogError(ex,
                        "Database migration failed after {MaxAttempts} attempts",
                        maxAttempts);
                    throw;
                }

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                logger.LogWarning(ex,
                    "Database migration attempt {Attempt}/{MaxAttempts} failed (SqlError {Number}: {Message}). Retrying in {Delay}...",
                    attempt, maxAttempts, ex.Number, ex.Message, delay);

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex,
                    "Database migration attempt {Attempt}/{MaxAttempts} failed: {Message}. Retrying...",
                    attempt, maxAttempts, ex.Message);

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)), cancellationToken);
            }
        }
    }

    private static bool IsTransient(SqlException ex)
    {
        return ex.Number switch
        {
            1801 => true,
            1205 => true,
            1222 => true,
            4060 => true,
            40197 => true,
            40501 => true,
            40613 => true,
            49918 => true,
            49919 => true,
            49920 => true,
            11001 => true,
            10928 => true,
            10929 => true,
            10053 => true,
            10054 => true,
            10060 => true,
            233 => true,
            64 => true,
            20 => true,
            -2 => true,
            _ => false
        };
    }
}
